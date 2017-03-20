namespace TestApi.Controllers
{
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Library.Bases;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;

    [Route(LecturerController.DefaultApiControllerRoute)]
    public class LecturerController : SecureControllerBase
    {
        public LecturerController(Entities context, MyJsonSerializerSettings settings)
            : base(context, settings)
        {
        }

        // GET api/values/5
        [HttpGet(LecturerController.ApiRouteIdSegment)]
        public IActionResult Get(int id)
        {
            LecturerPerson lecturer = this.Context.LecturerPersons
                .Include(s => s.Person)
                .Include(s => s.Lecturer)
                .SingleOrDefault(s => s.LecturerId == id);

            if (lecturer == null)
            {
                return HttpBadRequest();
            }

            return Json(lecturer, this.Settings);
        }
    }
}