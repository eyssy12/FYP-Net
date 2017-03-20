namespace CMS.Dashboard.Test.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Dashboard.Test.Serialization;
    using CMS.Dashboard.Test.ViewModels.Configuration;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;

    [Authorize(Roles = "Student,Lecturer,Admin")]
    [Produces("application/json")]
    [Route("api/Students")]
    public class StudentsController : Controller
    {
        private readonly Entities Context;
        private readonly MyJsonSerializerSettings Settings;

        public StudentsController(Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Settings = settings;
        }

        // GET: api/Students
        [HttpGet]
        public JsonResult GetStudents()
        {
            IEnumerable<Student> results = this.Context.Students
                .Include(s => s.Class)
                .Include(s => s.StudentPerson)
                    .ThenInclude(s => s.Person)
                .ToArray();

            return Json(results, this.Settings);
        }

        // GET: api/Students/5
        [HttpGet("{id}", Name = "GetStudent")]
        public IActionResult GetStudent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Student student = Context.Students.Single(m => m.Id == id);

            if (student == null)
            {
                return HttpNotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, [FromBody] StudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            Student student = this.Context.Students
                .Include(s => s.Class)
                .Include(s => s.StudentPerson).ThenInclude(s => s.Person)
                .SingleOrDefault(s => s.Id == viewModel.Id);

            student.EnrollmentDate = viewModel.EnrollmentDate;

            if (student.Class.Id != viewModel.ClassId)
            {
                Class @class = this.Context.Classes.SingleOrDefault(s => s.Id == viewModel.ClassId);

                student.Class = @class;
            }

            this.Context.Entry(student).State = EntityState.Modified;

            try
            {
                Context.SaveChanges();

                return Json(student, this.Settings);
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        // POST: api/Students
        [HttpPost]
        public IActionResult PostStudent([FromBody] StudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Class @class = this.Context.Classes.SingleOrDefault(s => s.Id == viewModel.ClassId);

            Person person = this.Context.Persons.SingleOrDefault(s => s.Id == viewModel.PersonId);

            Student student = new Student
            {
                Class = @class,
                EnrollmentDate = viewModel.EnrollmentDate
            };

            StudentPerson studentPerson = new StudentPerson
            {
                Person = person,
                PersonId = person.Id,
                StudentId = student.Id
            };

            student.StudentPerson = studentPerson;
            
            try
            {
                this.Context.Students.Add(student);
                this.Context.SaveChanges();

                return Json(student, this.Settings);
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Student student= this.Context.Students
                 .Include(s => s.StudentPerson)
                 .Include(s => s.StudentModules)
                 .Single(m => m.Id == id);

            if (student == null)
            {
                return HttpNotFound();
            }

            try
            {
                this.Context.StudentModules.RemoveRange(student.StudentModules);
                this.Context.Students.Remove(student);

                this.Context.SaveChanges();

                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return HttpBadRequest();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(int id)
        {
            return Context.Students.Count(e => e.Id == id) > 0;
        }
    }
}