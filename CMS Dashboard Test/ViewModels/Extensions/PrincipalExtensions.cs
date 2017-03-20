namespace CMS.Dashboard.Test.ViewModels.Extensions
{
    using System.Security.Claims;
    using System.Security.Principal;

    public static class PrincipalExtensions
    {
        public static string FirstName(this IPrincipal user)
        {
            return user.GetClaim("FirstName");
        }

        public static string LastName(this IPrincipal user)
        {
            return user.GetClaim("LastName");
        }

        public static string FullName(this IPrincipal user)
        {
            return user.GetClaim("FullName");
        }

        public static string EnrollmentDate(this IPrincipal user)
        {
            return user.GetClaim("EnrollmentDate");
        }

        public static string LecturerHireDate(this IPrincipal user)
        {
            return user.GetClaim("HireDate");
        }

        private static string GetClaim(this IPrincipal user, string type)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == type)
                        return claim.Value;
                }
                return "Error";
            }
            else
            {
                return "Error";
            }
        }
    }
}
