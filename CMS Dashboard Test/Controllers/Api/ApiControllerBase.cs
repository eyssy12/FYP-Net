namespace CMS.Dashboard.Test.Controllers.Api
{
    using System.Linq;
    using System.Security.Principal;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;
    using Serialization;

    public abstract class ApiControllerBase<TEntity> : Controller
        where TEntity : class
    {
        protected readonly Entities Context;
        protected readonly ILogger Logger;
        protected readonly MyJsonSerializerSettings Settings;

        protected ApiControllerBase(ILoggerFactory loggerFactory, Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Logger = loggerFactory.CreateLogger<AccountController>();
            this.Settings = settings;
        }

        protected ApplicationUser GetCurrentApplicationUserFromPrincipal()
        {
            IIdentity identity = this.User.Identity;

            ApplicationUser user = this.Context.Users
                .Include(u => u.Person)
                .Include(u => u.Roles)
                .SingleOrDefault(s => s.UserName == identity.Name);

            return user;
        }
    }
}