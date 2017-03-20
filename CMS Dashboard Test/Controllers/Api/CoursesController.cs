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
    using Newtonsoft.Json;
    using Serialization;
    using ViewModels;
    using ViewModels.Configuration;

    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/Courses")]
    public class CoursesController : Controller
    {
        private readonly Entities Context;
        private readonly JsonSerializerSettings Settings;

        public CoursesController(Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Settings = settings;
        }

        // GET: api/Courses
        [HttpGet]
        public IActionResult GetCourses()
        {
            IEnumerable<Course> courses = this.Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Semesters)
                    .ThenInclude(s => s.Classes)
                        .ThenInclude(s => s.Students)
                .Include(c => c.Semesters)
                    .ThenInclude(s => s.Classes)
                        .ThenInclude(s => s.Timetable)
                            .ThenInclude(s => s.Events)
                .Include(s => s.Semesters).ThenInclude(S => S.Modules)
                .ToArray();

            return Json(courses, this.Settings);
        }

        // GET: api/Courses/5
        [HttpGet("{id}", Name = "GetCourse")]
        public IActionResult GetCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Course course = this.Context.Courses.Include(c => c.Department).Single(m => m.Id == id);

            if (course == null)
            {
                return HttpNotFound();
            }

            return Ok(course);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public IActionResult PutCourse(int id, [FromBody] CourseViewModel courseViewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != courseViewModel.Id)
            {
                return HttpBadRequest();
            }

            Course course = this.Context.Courses.Include(c => c.Department).SingleOrDefault(c => c.Id == id);

            if (course.Name != courseViewModel.Name)
            {
                if (this.Context.Courses.Any(d => d.Name == courseViewModel.Name))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }

                course.Name = courseViewModel.Name;
            }

            if (course.Code != courseViewModel.Code)
            {
                if (this.Context.Courses.Any(d => d.Code == courseViewModel.Code))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }

                course.Code = courseViewModel.Code;
            }

            course.AwardType = courseViewModel.AwardType;

            Department department = this.Context.Departments.SingleOrDefault(d => d.Id == courseViewModel.DepartmentId);

            if (course.Department == null || course.Department.Name != department.Name)
            {
                department.Courses.Add(course);
            }

            Context.Entry(course).State = EntityState.Modified;

            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/Courses
        [HttpPost]
        public IActionResult PostCourse([FromBody] CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (!this.Context.Courses.Any(d => d.Name == viewModel.Name))
            {
                Department department = this.Context.Departments.SingleOrDefault(d => d.Id == viewModel.DepartmentId);

                Course course = new Course
                {
                   AwardType = viewModel.AwardType,
                   Code = viewModel.Code,
                   Department = department,
                   Name = viewModel.Name
                };

                department.Courses.Add(course);

                this.Context.Courses.Add(course); 

                try
                {
                    Context.SaveChanges();
                }
                catch
                {
                    return HttpBadRequest(ModelState);
                }

                Response.StatusCode = StatusCodes.Status201Created;

                return Json(course, this.Settings);
            }

            return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
        }

        [Route("bulkDelete")]
        [HttpPost]
        public IActionResult BulkDelete([FromBody] IdArrayViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            IEnumerable<Course> courses = this.Context.Courses.Where(d => viewModel.Ids.Any(i => i == d.Id)).ToArray();

            if (courses.Any())
            {
                this.Context.Courses.RemoveRange(courses);
                this.Context.SaveChanges();

                return Ok();
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")] 
        public IActionResult DeleteCourse(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Course course = Context.Courses.SingleOrDefault(m => m.Id == id);
            if (course == null)
            {
                return HttpNotFound();
            }

            Context.Courses.Remove(course);
            Context.SaveChanges();

            return Ok(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int id)
        {
            return Context.Courses.Count(e => e.Id == id) > 0;
        }
    }
}