namespace CMS.Dashboard.Test.Controllers.Api
{
    using System;
    using Managers;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using DegreeAwardEnum = CMS.Web.Configuration.Data.Enumerations.DegreeAward;
    using EnrollmentStageEnum = CMS.Web.Configuration.Data.Enumerations.EnrollmentStage;
    using ModuleTypeEnum = CMS.Web.Configuration.Data.Enumerations.ModuleType;
    using UserRoleEnum = CMS.Web.Configuration.Data.Enumerations.UserRole;

    [Authorize(Roles = "Admin,Student,Lecturer")]
    [Produces("application/json")]
    [Route("api/Enumerations")]
    public class EnumerationsController : Controller
    {
        protected const string UserRolesTemplate = "UserRoles",
            EnrollmentStagesTemplate = "EnrollmentStages",
            DegreeAwardsTemplate = "DegreeAwards",
            ModuleTypesTemplate = "ModuleTypes";

        // GET: api/enumerations/UserRoles
        [Route(EnumerationsController.UserRolesTemplate)]
        [HttpGet]
        public IActionResult GetUserRoles()
        {
            return this.CreateResponse<UserRoleEnum>();
        }

        // GET: api/enumerations/EnrollmentStages
        [Route(EnumerationsController.EnrollmentStagesTemplate)]
        [HttpGet]
        public IActionResult GetEnrollmentStages()
        {
            return this.CreateResponse<EnrollmentStageEnum>();
        }

        // GET: api/enumerations/degreeawardss
        [Route(EnumerationsController.DegreeAwardsTemplate)]
        [HttpGet]
        public IActionResult GetDegreeAwards()
        {
            return this.CreateResponse<DegreeAwardEnum>();
        }

        // GET: api/enumerations/moduletypes
        [Route(EnumerationsController.ModuleTypesTemplate)]
        [HttpGet]
        public IActionResult GetModuleTypes()
        {
            return this.CreateResponse<ModuleTypeEnum>();
        }

        protected IActionResult CreateResponse<TEnum>()
            where TEnum : struct, IConvertible
        {
            return Json(EnumerationsExtensions.CreateEnumerationCollection<TEnum>());
        }
    }
}