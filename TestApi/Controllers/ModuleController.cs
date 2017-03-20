namespace TestApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Library.Bases;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;

    [Route(ModuleController.DefaultApiControllerRoute)]
    public class ModuleController : SecureControllerBase
    {
        protected const string ModulesForStudentTemplate = "ForStudent/" + SecureControllerBase.ApiRouteIdSegment,
            ModulesForLecturerTemplate = "ForLecturer/" + SecureControllerBase.ApiRouteIdSegment;

        public ModuleController(Entities context, MyJsonSerializerSettings settings)
            : base(context, settings)
        {}

        [Route(ModuleController.ModulesForStudentTemplate)]
        public IActionResult GetModulesForStudent(int id)
        {
            Class @class = this.Context.Classes
                .Include(s => s.Semester)
                    .ThenInclude(s => s.Modules)
                        .ThenInclude(s => s.LecturerModules)
                            .ThenInclude(s => s.Lecturer)
                                .ThenInclude(s => s.LecturerPerson)
                                    .ThenInclude(s => s.Person)
                .Include(s => s.Students)
                .SingleOrDefault(s => s.Students.Any(e => e.Id == id));

            if (@class == null)
            {
                return Json(new { });
            }
            // remove unneeded data so that it doesnt get sent accross, reduces bandwidth expense
            @class.Semester.Classes = null;
            @class.Students = null;
            foreach (Module module in @class.Semester.Modules)
            {
                module.Semester = null;
            }

            return Json(@class, this.Settings);
        }

        [Route(ModuleController.ModulesForLecturerTemplate)]
        public IActionResult GetModulesForLecturer(int id)
        {
            IEnumerable<Module> modules = this.Context.LecturerModules
                .Include(s => s.Lecturer)
                .Include(s => s.Module)
                .Where(s => s.LecturerId == id)
                .Select(s => s.Module)
                .ToArray();

            Response.StatusCode = StatusCodes.Status200OK;
            return Json(modules, this.Settings);
        }
    }
}