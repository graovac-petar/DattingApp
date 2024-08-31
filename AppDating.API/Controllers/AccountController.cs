using AppDating.API.Data;
using AppDating.API.DTO;
using AppDating.API.Helpers;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username))
                return BadRequest("Username already exists");

            var user = mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.Username;

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new AppUserDTO
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                knownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.Username.ToUpper());

            if (user == null) return Unauthorized("Invalid username");

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result) return Unauthorized("Invalid password");

            return new AppUserDTO
            {
                Username = user.UserName,
                knownAs = user.KnownAs,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender,
            };

        }


        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }


    }
}
