using CoreAppAuthDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Factory
{
    public class CustomClaimsFactory : UserClaimsPrincipalFactory<User>
    {
        public CustomClaimsFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("Username", user.UserName));
            identity.AddClaim(new Claim("FullName", user.FirstName + user.LastName));
            identity.AddClaim(new Claim("PhoneNumber", user.PhoneNumber));

            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            return identity;
        }
    }
}
