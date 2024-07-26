using AppDating.API.Data;
using AppDating.API.Model.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly DataContext _context;

        public AppUsersController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllAppUsers()
        {
            var users = await _context.AppUsers.ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetAppUser(int id)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
