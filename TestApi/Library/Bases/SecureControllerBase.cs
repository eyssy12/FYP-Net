namespace TestApi.Library.Bases
{
    using CMS.Web.Configuration.Data;
    using Microsoft.AspNet.Authentication.JwtBearer;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Serialization;

    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [RequireHttps]
    public abstract class SecureControllerBase : Controller
    {
        protected const string DefaultApiControllerRoute = "api/[controller]",
            ApiRouteIdSegment = "{id}";

        protected readonly Entities Context;
        protected readonly MyJsonSerializerSettings Settings;

        protected SecureControllerBase(Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Settings = settings;
        }
    }
}