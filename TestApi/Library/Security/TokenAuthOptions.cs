namespace TestApi.Library.Security
{
    using System.IdentityModel.Tokens;

    public class TokenAuthOptions
    {
        public string Audience { get; set; }

        public string Issuer { get; set; }

        public SigningCredentials SigningCredentials { get; set; }
    }
}