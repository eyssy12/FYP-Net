namespace CMS.Dashboard.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;
    using ViewModels.Configuration;

    [Authorize(Roles = "Admin,Student")]
    [Produces("application/json")]
    [Route("api/Semesters")]
    public class SemestersController : Controller
    {
        private readonly Entities Context;
        private readonly MyJsonSerializerSettings Settings;

        public SemestersController(Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Settings = settings;
        }

        // GET: api/Semesters
        [HttpGet]
        public IActionResult GetSemesters()
        {
            IEnumerable<Semester> semesters = this.Context.Semesters
                .Include(s => s.Course)
                .Include(s => s.Classes)
                .Include(s => s.Modules).ToArray();

            IEnumerable<Semester> obsoleteSemesters = semesters.Where(s => s.Course == null).ToArray();
            if (obsoleteSemesters.Any())
            {
                this.Context.Semesters.RemoveRange(obsoleteSemesters);
                this.Context.SaveChanges();
            }

            return Json(semesters, this.Settings);
        }

        // GET: api/Semesters/5
        [HttpGet("{id}", Name = "GetSemester")]
        public IActionResult GetSemester([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Semester semester = Context.Semesters.Single(m => m.Id == id);

            if (semester == null)
            {
                return HttpNotFound();
            }

            return Ok(semester);
        }

        // PUT: api/Semesters/5
        [HttpPut("{id}")]
        public IActionResult PutSemester(int id, [FromBody] SemesterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            Semester semester = this.Context.Semesters.Include(s => s.Course).SingleOrDefault(s => s.Id == id);

            semester.Number = viewModel.Number;

            if (semester.Course == null || semester.Course.Id != viewModel.CourseId)
            {
                semester.Course = this.Context.Courses.SingleOrDefault(s => s.Id == viewModel.CourseId);
            }

            Context.Entry(semester).State = EntityState.Modified;

            try
            {
                Context.SaveChanges();

                Response.StatusCode = StatusCodes.Status201Created;

                return Json(semester, this.Settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new HttpStatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        // POST: api/Semesters
        [HttpPost]
        public IActionResult PostSemester([FromBody] SemesterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Course course = this.Context.Courses.Single(c => c.Id == viewModel.CourseId);

            Semester semester = new Semester
            {
                Number = viewModel.Number,
                Course = course
            };

            this.Context.Semesters.Add(semester);

            try
            {
                this.Context.SaveChanges();

                Response.StatusCode = StatusCodes.Status201Created;

                return Json(semester, this.Settings);
            }
            catch (DbUpdateException)
            {
                return HttpBadRequest(ModelState);
            }
        }

        // DELETE: api/Semesters/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSemester(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Semester semester = Context.Semesters.Single(m => m.Id == id);
            if (semester == null)
            {
                return HttpNotFound();
            }

            Context.Semesters.Remove(semester);
            Context.SaveChanges();

            return Ok(semester);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SemesterExists(int id)
        {
            return Context.Semesters.Count(e => e.Id == id) > 0;
        }
    }
}