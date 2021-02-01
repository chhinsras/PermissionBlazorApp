using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBlazorApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PermissionBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> ListUsers()
        {
            // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            //var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
            var allUsers = await _userManager.Users.ToListAsync();

            List<UserDto> users = new List<UserDto>();

            foreach(var user in allUsers)
            {
                users.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName
                });
            }

            return Ok(users);
        }
    }
}
