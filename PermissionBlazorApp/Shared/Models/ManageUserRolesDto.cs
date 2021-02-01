using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionBlazorApp.Shared.Models
{
    public class ManageUserRolesDto
    {
        public string UserId { get; set; }
        public IList<UserRolesDto> UserRoles { get; set; }
    }
}