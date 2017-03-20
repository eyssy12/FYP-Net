namespace TestApi.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Library;
    using Library.Security;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Models;
    using Newtonsoft.Json.Linq;
    using UserRole = CMS.Web.Configuration.Data.Enumerations.UserRole;

    [Route("api/[controller]")]
    [RequireHttps]
    public class AuthController : Controller
    {
        // in minutes
        protected const int DefaultIdTokenExpiryTime = 300,
            DefaultAccessTokenExpiryTime = 20160;

        protected const string NewIdTokenTemplate = "NewIdToken",
            ClaimEntityId = "EntityID",
            ClaimEntityAuthTokenType = "AuthToken";

        private readonly Entities Context;
        private readonly TokenAuthOptions TokenOptions;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly TokenStorageService TokenStorage;

        public AuthController(Entities context, TokenAuthOptions tokenOptions, UserManager<ApplicationUser> userManager, TokenStorageService tokenStorage)
        {
            this.Context = context;
            this.TokenOptions = tokenOptions;
            this.UserManager = userManager;
            this.TokenStorage = tokenStorage;
        }

        [Route(AuthController.NewIdTokenTemplate)]
        public dynamic GetNewIdToken([FromBody] RefreshTokenRequest request)
        {
            RefreshToken token = this.Context.RefreshTokens.Include(s => s.User).SingleOrDefault(s => s.Token == request.AccessToken);

            if (token == null)
            {
                return new
                {
                    data_type = "error",
                    error_message = "Invalid refresh token"
                };
            }

            JObject responseObject = new JObject();
            DateTime expiryDate = DateTime.UtcNow.AddMinutes(5);
            string newIdToken = this.GenerateToken(token.User, expiryDate);

            responseObject.Add(Protocol.IdToken, newIdToken);
            responseObject.Add(Protocol.IdTokenExpiry, expiryDate);

            return responseObject;
        }
        
        [HttpPost]
        public async Task<dynamic> Authenticate([FromBody] AuthRequest request)
        {
            if (request == null)
            {
                return new
                {
                    data_type = "error",
                    error_message = "No data provided"
                };
            }

            ApplicationUser user = this.Context.Users
                .Include(s => s.Person)
                .SingleOrDefault(s => s.UserName == request.Username);

            if (user != null)
            {
                bool isValid = await this.UserManager.CheckPasswordAsync(user, request.Password);

                if (isValid)
                {
                    return this.ReturnTokenForAuthenticatedEntity(user);
                }
            }

            return new { authenticated = false };
        }

        private bool IsRefreshTokenRequired(ApplicationUser user, out RefreshToken outToken)
        {
            // need to ensure we create a refresh token for first time authenticators, so we check for existing
            RefreshToken refreshToken = this.Context.RefreshTokens.Include(s => s.User).SingleOrDefault(s => s.User.Id == user.Id);

            // first time authentication
            if (refreshToken == null)
            {
                DateTime expires = DateTime.UtcNow.AddMinutes(AuthController.DefaultAccessTokenExpiryTime);
                string token = this.GenerateToken(user, expires);

                refreshToken = new RefreshToken
                {
                    User = user,
                    ExpiryDate = expires,
                    Token = token
                };

                this.Context.RefreshTokens.Add(refreshToken);
                this.Context.SaveChanges();

                outToken = refreshToken;

                return true;
            }

            // not the first time but reauthentication -> we have an existing record in the db
            if (DateTime.UtcNow > refreshToken.ExpiryDate)
            {
                DateTime expires = DateTime.UtcNow.AddMinutes(AuthController.DefaultAccessTokenExpiryTime);
                string token = this.GenerateToken(user, expires);

                refreshToken.ExpiryDate = expires;
                refreshToken.Token = token;
                
                this.Context.Entry(refreshToken).State = EntityState.Modified;
                this.Context.SaveChanges();

                outToken = refreshToken;

                return true;
            }

            // not the first time nor is token out of date
            outToken = refreshToken;

            return false;
        }

        protected dynamic ReturnTokenForAuthenticatedEntity(ApplicationUser user)
        {
            JObject responseObject = new JObject();
            this.DetermineUserType(user, responseObject);

            RefreshToken refreshToken;
            if (this.IsRefreshTokenRequired(user, out refreshToken))
            {
                return this.ReturnTokenForAuthenticatedEntity(user, responseObject, token => this.TokenStorage.AddToken(token), refreshToken: refreshToken);
            }

            StorageToken existingToken = this.TokenStorage.GetToken(user.Id);
            if (existingToken == null)
            {
                return this.ReturnTokenForAuthenticatedEntity(user, responseObject, token => this.TokenStorage.AddToken(token), refreshToken: refreshToken);
            }

            if (DateTime.UtcNow > existingToken.ExpirationTime)
            {
                return this.ReturnTokenForAuthenticatedEntity(user, responseObject, token => this.TokenStorage.Replace(token), refreshToken: refreshToken);
            }

            return this.ReturnTokenForAuthenticatedEntity(responseObject, existingToken, refreshToken: refreshToken);
        }

        protected dynamic ReturnTokenForAuthenticatedEntity(ApplicationUser user, JObject responseObject, Action<StorageToken> storageServiceOperation, RefreshToken refreshToken = null)
        {
            StorageToken newToken = this.CreateToken(user);

            storageServiceOperation(newToken);

            return this.ReturnTokenForAuthenticatedEntity(responseObject, newToken, refreshToken: refreshToken);
        }

        protected dynamic ReturnTokenForAuthenticatedEntity(JObject responseObject, StorageToken token, RefreshToken refreshToken = null)
        {
            responseObject.Add(Protocol.Authenticated, true);
            responseObject.Add(Protocol.EntityId, token.EntityId);
            responseObject.Add(Protocol.IdToken, token.Token);
            responseObject.Add(Protocol.IdTokenExpiry, token.ExpirationTime);

            if (refreshToken != null)
            {
                responseObject.Add(Protocol.AccessToken, refreshToken.Token);
                responseObject.Add(Protocol.AccessTokenExpiry, refreshToken.ExpiryDate);
            }

            return responseObject;
        }

        private void DetermineUserType(ApplicationUser user, JObject responseObject)
        {
            StudentPerson student = this.Context.StudentPersons
                .Include(s => s.Student)
                .SingleOrDefault(s => s.PersonId == user.Person.Id);

            if (student == null)
            {
                LecturerPerson lecturer = this.Context.LecturerPersons
                    .Include(s => s.Lecturer)
                    .SingleOrDefault(s => s.PersonId == user.Person.Id);

                if (lecturer == null)
                {
                    responseObject.Add(Protocol.UserType, UserRole.Admin.ToString().ToLower());
                }
                else
                {
                    responseObject.Add(Protocol.LecturerId, lecturer.LecturerId);
                    responseObject.Add(Protocol.UserType, UserRole.Lecturer.ToString().ToLower());
                }
            }
            else
            {
                responseObject.Add(Protocol.StudentId, student.StudentId);
                responseObject.Add(Protocol.UserType, UserRole.Student.ToString().ToLower());
            }
        }

        private StorageToken CreateToken(ApplicationUser user)
        {
            DateTime expires = DateTime.UtcNow.AddMinutes(AuthController.DefaultIdTokenExpiryTime);

            string token = this.GenerateToken(user, expires);

            return new StorageToken(user.Id, token, expires);
        }

        private string GenerateToken(ApplicationUser user, DateTime expires)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            // this will form a part of the token so that when the request comes in the controller will be able to parse it out and interpet who is the user from the claims identity
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.UserName, AuthController.ClaimEntityAuthTokenType),
                new[] 
                {
                    new Claim(AuthController.ClaimEntityId, user.Id, ClaimValueTypes.String)
                });

            JwtSecurityToken securityToken = handler.CreateToken(
                issuer: this.TokenOptions.Issuer,
                audience: this.TokenOptions.Audience,
                signingCredentials: this.TokenOptions.SigningCredentials,
                subject: identity,
                expires: expires);

            return handler.WriteToken(securityToken);
        }
    }
}