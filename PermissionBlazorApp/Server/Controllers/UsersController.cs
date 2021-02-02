using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBlazorApp.Server.Models;
using PermissionBlazorApp.Server.Services;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;

        public UsersController(UserManager<AppUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
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

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenAsync(TokenRequestModel model)
        {
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }
    }
}
