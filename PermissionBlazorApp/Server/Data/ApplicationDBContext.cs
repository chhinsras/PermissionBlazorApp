using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PermissionBlazorApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionBlazorApp.Server.Data
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser, IdentityRole, string,
                    IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>,
                    IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
