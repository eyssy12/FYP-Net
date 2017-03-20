namespace CMS.Dashboard.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;
    using Serialization;
    using ViewModels.Configuration;

    [Produces("application/json")]
    [Route("api/Classes")]
    public class ClassesController : ApiControllerBase<Class>
    {
        public ClassesController(ILoggerFactory loggerFactory, Entities context, MyJsonSerializerSettings settings)
            : base(loggerFactory, context, settings)
        {
        }

        [Route("GetClassForStudent")]
        public IActionResult GetClassForStudent()
        {
            ApplicationUser user = this.GetCurrentApplicationUserFromPrincipal();

            StudentPerson thisStudentPerson = this.Context.StudentPersons
                .Include(s => s.Student)
                .Include(s => s.Person)
                .SingleOrDefault(s => s.PersonId == user.Person.Id);

            IEnumerable<Class> classes = this.Context.Classes
                .Include(s => s.Students)
                    .ThenInclude(s => s.StudentPerson)
                        .ThenInclude(s => s.Student)
                .Include(s => s.Students)
                    .ThenInclude(s => s.StudentPerson)
                        .ThenInclude(s => s.Person)
                .ToArray();

            Student thisStudent = classes.SelectMany(s => s.Students).SingleOrDefault(s => s.StudentPerson.StudentId == thisStudentPerson.StudentId);

            thisStudent.Class.Students.Remove(thisStudent);

            return Json(thisStudent.Class, this.Settings);
        }

        // GET: api/Classes
        [HttpGet]
        public IActionResult GetClasses()
        {
            IEnumerable<Class> classes = this.Context.Classes
                .Include(c => c.Semester)
                    .ThenInclude(c => c.Course)
                .Include(s => s.Timetable)
                .Include(c => c.Students);

            return Json(classes, this.Settings);
        }

        // GET: api/Classes/5
        [HttpGet("{id}", Name = "GetClass")]
        public IActionResult GetClass([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Class @class = this.Context.Classes.Single(m => m.Id == id);

            if (@class == null)
            {
                return HttpNotFound();
            }

            return Ok(@class);
        }

        // PUT: api/Classes/5
        [HttpPut("{id}")]
        public IActionResult PutClass(int id, [FromBody] ClassViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            Class @class = this.Context.Classes.Include(s => s.Semester).SingleOrDefault(s => s.Id == viewModel.Id);
            if (@class.Name != viewModel.Name)
            {
                @class.Name = viewModel.Name;
            }
            
            if (@class.Semester.Id != viewModel.SemesterId)
            {
                Semester semester = this.Context.Semesters.SingleOrDefault(s => s.Id == viewModel.SemesterId);
                @class.Semester = semester;
            }

            this.Context.Entry(@class).State = EntityState.Modified;

            try
            {
                this.Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Classes
        [HttpPost]
        public IActionResult PostClass([FromBody] ClassViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Semester semester = this.Context.Semesters.Include(s => s.Course).SingleOrDefault(s => s.Id == viewModel.SemesterId);

            Class @class = new Class
            {
                EnrollmentStage = viewModel.EnrollmentStage,
                Name = viewModel.Name,
                Semester = semester,
                YearCommenced = viewModel.YearCommenced,
            };

            try
            {
                this.Context.Classes.Add(@class);
                this.Context.SaveChanges();

                Response.StatusCode = StatusCodes.Status201Created;

                return Json(@class, this.Settings);
            }
            catch (DbUpdateException)
            {
                return HttpBadRequest();
            }
        }

        // DELETE: api/Classes/5
        [HttpDelete("{id}")]
        public IActionResult DeleteClass(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Class @class = this.Context.Classes.Single(m => m.Id == id);
            if (@class == null)
            {
                return HttpNotFound();
            }

            this.Context.Classes.Remove(@class);
            this.Context.SaveChanges();

            return Ok(@class);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClassExists(int id)
        {
            return this.Context.Classes.Count(e => e.Id == id) > 0;
        }
    }
}