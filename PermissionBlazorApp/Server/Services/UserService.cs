using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PermissionBlazorApp.Server.Models;
using PermissionBlazorApp.Server.Settings;
using PermissionBlazorApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PermissionBlazorApp.Server.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JWT _jwt;

        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwt = jwt.Value;
        }

        //public async Task<AuthenticationModel> GetingTokenAsync(TokenRequestModel model)
        //{
        //    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
        //    if (result.Succeeded)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);

        //        // Get valid claims and pass them into JWT
        //        var claims = await GetValidClaims(user);

        //        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        //        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        //        // Create the JWT security token and encode it.
        //        var jwt = new JwtSecurityToken(
        //            issuer: _jwt.Issuer,
        //            audience: _jwt.Audience,
        //            claims: claims,
        //           expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
        //        signingCredentials: signingCredentials);
        //        //...
        //    }
        //    else
        //    {
        //        throw new ApiException('Wrong username or password', 403);
        //    }
        //}

        //private async Task<List<Claim>> GetValidClaims(AppUser user)
        //{
        //    IdentityOptions _options = new IdentityOptions();
           
        //    var userClaims = await _userManager.GetClaimsAsync(user);
        //    var userRoles = await _roleManager.GetClai(user);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim("uid", user.Id)
        //    }
        //    .Union(userClaims)
        //    .Union(roleClaims);


        //    claims.AddRange(userClaims);
        //    foreach (var userRole in userRoles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, userRole));
        //        var role = await _roleManager.FindByNameAsync(userRole);
        //        if (role != null)
        //        {
        //            var roleClaims = await _roleManager.GetClaimsAsync(role);
        //            foreach (Claim roleClaim in roleClaims)
        //            {
        //                claims.Add(roleClaim);
        //            }
        //        }
        //    }
        //    return claims;
        //}

        public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
        {
            var authenticationModel = new AuthenticationModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"No Accounts Registered with {model.Email}.";
                return authenticationModel;
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authenticationModel.IsAuthenticated = true;
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.Email = user.Email;
                authenticationModel.UserName = user.UserName;
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();
                return authenticationModel;
            }
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Incorrect Credentials for user {user.Email}.";
            return authenticationModel;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

           // var userRoles = await _userManager.GetClaimsAsync(user);

            //var roleClaims = new List<Claim>();

            //for (int i = 0; i < roles.Count; i++)
            //{
            //    roleClaims.Add(new Claim("roles", roles[i]));
            //    // roleClaims.Add(new Claim("Permission", permissions[i].Value));
            //}

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            };
            //.Union(userClaims)
            //.Union(roleClaims);

            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }




            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    
    
        
    
    }
}
