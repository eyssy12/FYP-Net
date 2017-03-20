namespace TestApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Library.Bases;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using TestApi.Serialization;

    [Route(StudentController.DefaultApiControllerRoute)]
    public class StudentController : SecureControllerBase
    {
        protected const string GetStudentTemplate = "GetStudent";

        public StudentController(Entities context, MyJsonSerializerSettings settings)
            : base(context, settings)
        {
        }

        // GET: api/student
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Student> students = this.Context.StudentModules.Include(s => s.Student).Select(s => s.Student);

            return Json(students, this.Settings);
        }

        // GET api/student/5
        [HttpGet(StudentController.ApiRouteIdSegment, Name = StudentController.GetStudentTemplate)]
        public IActionResult GetStudent(int id)
        {
            StudentPerson studentPerson = this.Context.StudentPersons
                .Include(s => s.Student)
                    .ThenInclude(s => s.Class)
                .Include(s => s.Person)
                .Include(s => s.Student)
                    .ThenInclude(s => s.StudentModules)
                        .ThenInclude(s => s.Module)
                .SingleOrDefault(s => s.StudentId == id);

            if (studentPerson == null)
            {
                return HttpNotFound();
            }

            return Json(studentPerson, this.Settings);
        }
    }
}