namespace TestApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Extensions;
    using Library.Bases;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;

    [Route(ClassController.DefaultApiControllerRoute)]
    public class ClassController : SecureControllerBase
    {
        protected const string ClassmatesForStudentTemplate = "ClassmatesForStudent/" + ClassController.ApiRouteIdSegment,
            ClassForStudentTemplate = "ClassForStudent/" + ClassController.ApiRouteIdSegment;

        public ClassController(Entities context, MyJsonSerializerSettings settings)
            : base(context, settings)
        {
        }

        [Route(ClassController.ClassmatesForStudentTemplate)]
        public IActionResult GetClassmatesForStudent(int id)
        {
            Class @class = this.Context.Classes.Include(s => s.Students).ThenInclude(s => s.StudentPerson).ThenInclude(s => s.Person).SingleOrDefault(s => s.Students.Any(e => e.Id == id));
            if (@class == null)
            {
                return Json(new { });
            }
            IEnumerable<StudentPerson> studentPersons = @class.Students.Select(s => s.StudentPerson);
            StudentPerson thisStudent = studentPersons.SingleOrDefault(s => s.StudentId == id);
            // should be one less from here, we dont want to return the same student back
            IEnumerable<StudentPerson> classmates = studentPersons.Except(new[] { thisStudent });
            classmates.ForEach(s => { s.Student.Class = null; });

            return Json(classmates, this.Settings);
        }

        [Route(ClassController.ClassForStudentTemplate)]
        public IActionResult GetClassForStudent(int id)
        {
            Class @class = this.Context.Classes.Include(s => s.Students).ThenInclude(s => s.StudentPerson).ThenInclude(s => s.Person).Include(s => s.Semester).SingleOrDefault(s => s.Students.Any(e => e.Id == id));
            if (@class == null)
            {
                return Json(new { });
            }

            return Json(@class, this.Settings);
        }
    }
}