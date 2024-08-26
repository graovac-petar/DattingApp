using AppDating.API.Data;
using AppDating.API.DTO;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DattingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username))
                return BadRequest("Username already exists");


            using var hmac = new HMACSHA512();

            var user = mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.Username;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;

            context.Add(user);
            await context.SaveChangesAsync();

            return new AppUserDTO
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                knownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await context.AppUsers.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Password not good");
            }

            return new AppUserDTO
            {
                Username = user.UserName,
                knownAs = user.KnownAs,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };

        }


        private async Task<bool> UserExists(string username)
        {
            return await context.AppUsers.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }


    }
}
