using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBlazorApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin")]
    [Authorize(Roles = "Basic")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> ListRoles()
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            
            List<RoleDto> roles = new List<RoleDto>();

            foreach (var role in allRoles)
            {
                roles.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return Ok(roles);
        }
        [HttpPost]
        public async Task<ActionResult> AddRole(RoleRequest roleRequest)
        {
            if (roleRequest.Name != null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleRequest.Name.Trim()));
            }
            return Ok();
        }
    }
}
