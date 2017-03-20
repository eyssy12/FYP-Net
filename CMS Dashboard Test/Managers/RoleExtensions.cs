namespace CMS.Dashboard.Test.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Views.Models;
    using Web.Configuration.Data.Models;
    using UserRoleEnum = Web.Configuration.Data.Enumerations.UserRole;

    public static class RoleExtensions
    {
        public static async void EnsureRolesCreated(this RoleManager<IdentityRole> roleManager)
        {
            IEnumerable<string> values = Enum.GetNames(typeof(UserRoleEnum)).ToArray();

            foreach (string value in values)
            {
                await roleManager.EnsureRoleCreated(value);
            }
        }

        private static async Task EnsureRoleCreated(this RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public static object GetRoleFromUser(this ApplicationUser user, IEnumerable<IdentityRole> roles)
        {
            IdentityUserRole<string> role = user.Roles.SingleOrDefault();

            IdentityRole identityRole = roles.SingleOrDefault(s => s.Id == role.RoleId);

            UserRoleEnum roleEnum;
            if (Enum.TryParse(identityRole.Name, out roleEnum))
            {
                return roleEnum;
            }

            return null; 
        }

        public static IEnumerable<EnumId> GetRoleAsEnumId(this ApplicationUser user, IEnumerable<IdentityRole> roles)
        {
            IdentityUserRole<string> role = user.Roles.SingleOrDefault();

            IdentityRole identityRole = roles.SingleOrDefault(s => s.Id == role.RoleId);

            UserRoleEnum roleEnum;
            if (Enum.TryParse(identityRole.Name, out roleEnum))
            {
                return new EnumId[] 
                {
                    new EnumId
                    {
                        Id = (int)roleEnum,
                        Value = roleEnum.ToString()
                    }
                };
            }

            return Enumerable.Empty<EnumId>();
        }
    }
}