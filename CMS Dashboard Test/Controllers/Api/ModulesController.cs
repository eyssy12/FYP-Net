namespace CMS.Dashboard.Test.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;
    using Serialization;
    using Test.Controllers.Api;
    using ViewModels.Configuration;

    [Authorize(Roles = "Lecturer,Admin,Student")]
    [Produces("application/json")]
    [Route("api/Modules")]
    public class ModulesController : ApiControllerBase<Module>
    {
        public ModulesController(ILoggerFactory loggerFactory, Entities context, MyJsonSerializerSettings settings)
            : base(loggerFactory, context, settings)
        {
        }

        // GET: api/Modules
        [HttpGet]
        public IActionResult GetModules()
        {
            IEnumerable<Module> modules = this.Context.Modules
                .Include(m => m.Semester)
                    .ThenInclude(s => s.Course)
                .Include(m => m.Semester)
                    .ThenInclude(s => s.Classes)
                .Include(m => m.LecturerModules)
                    .ThenInclude(lm => lm.Lecturer)
                .Include(m => m.StudentModules)
                    .ThenInclude(sm => sm.Student);

            return Json(modules, this.Settings);
        }

        [Route("ModulesAllInclusive")]
        public IActionResult GetModulesAllInclusive()
        {
            IEnumerable<Module> modules = this.Context.Modules
                .Include(m => m.Semester).ThenInclude(s => s.Modules)
                .Include(m => m.LecturerModules).ThenInclude(lm => lm.Lecturer)
                .Include(m => m.StudentModules).ThenInclude(sm => sm.Student);

            return Json(modules, this.Settings);
        }

        [Route("GetModulesForStudent")]
        public IActionResult GetModulesForStudent()
        {
            ApplicationUser user = this.GetCurrentApplicationUserFromPrincipal();

            StudentPerson thisStudent = this.Context.StudentPersons
                .Include(s => s.Student)
                .SingleOrDefault(s => s.PersonId == user.Person.Id);

            Class @class = this.Context.Classes
                .Include(s => s.Students)
                .Include(s => s.Semester)
                    .ThenInclude(s => s.Modules)
                        .ThenInclude(s => s.LecturerModules)
                .ToArray()
                .SingleOrDefault(s => s.Students.Contains(thisStudent.Student)); // a student can only be in a single class, so we're guaranteed to get one or nothing

            if (@class == null)
            {
                return Json(new { }, this.Settings);
            }

            var temp = new
            {
                Modules = @class.Semester.Modules,
                Students = @class.Students
            };

            return Json(temp, this.Settings);
        }

        [Route("GetModulesForLecturer")]
        public IActionResult GetModulesForLecturer()
        {
            ApplicationUser user = this.GetCurrentApplicationUserFromPrincipal();

            LecturerPerson thisLecturer = this.Context.LecturerPersons
                .Include(s => s.Lecturer)
                .SingleOrDefault(s => s.PersonId == user.Person.Id);

            IEnumerable<LecturerModule> lecturerModules = this.Context.LecturerModules
                .Include(s => s.Module)
                .Where(s => s.LecturerId == thisLecturer.LecturerId)
                .ToArray();

            IEnumerable<Module> modules = lecturerModules
                .Distinct()
                .Where(s => s.LecturerId == thisLecturer.LecturerId)
                .Select(s => s.Module)
                .ToArray();

            return Json(modules, this.Settings);
        }

        // GET: api/Modules/5
        [HttpGet("{id}", Name = "GetModule")]
        public IActionResult GetModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Module module = this.Context.Modules
                .Include(s => s.Semester)
                .Single(m => m.Id == id);

            if (module == null)
            {
                return HttpNotFound();
            }

            this.Response.StatusCode = StatusCodes.Status200OK;

            return Json(module, this.Settings);
        }

        // PUT: api/Modules/5
        [HttpPut("{id}")]
        public IActionResult PutModule(int id, [FromBody] ModuleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            Module module = this.Context.Modules
                .Include(s => s.LecturerModules)
                    .ThenInclude(s => s.Module)
                .Include(s => s.Semester)
                    .ThenInclude(s => s.Course)
                        .ThenInclude(s => s.Department)
                .SingleOrDefault(s => s.Id == viewModel.Id);

            // TODO:should follow the same approach i went with the timetables -> different collections sent across

            // get the existing lectures assigned already in the module
            IEnumerable<LecturerModule> existing = module.LecturerModules.ToArray();

            //  this is the collection i want to end up with in the database
            IEnumerable<LecturerPerson> lecturers = this.Context.LecturerPersons
                .Include(s => s.Lecturer)
                .Include(s => s.Person)
                .Where(s => viewModel.LecturerIds.Contains(s.LecturerId))
                .ToArray();
            
            IEnumerable<Lecturer> toAdd = lecturers
                .Select(s => s.Lecturer)
                .Except(existing.Select(s => s.Lecturer))
                .ToArray();

            if (toAdd.Any())
            {
                IEnumerable<LecturerModule> lecturerModules = toAdd.Select(lecturer => new LecturerModule
                {
                    LecturerId = lecturer.Id,
                    Lecturer = lecturer,
                    Module = module,
                    ModuleId = module.Id
                });

                foreach (LecturerModule newLecturerModule in lecturerModules)
                {
                    module.LecturerModules.Add(newLecturerModule);
                }
            }
            else
            {
                IEnumerable<LecturerModule> toRemove = existing
                    .Where(s => !lecturers
                        .Select(e => e.LecturerId)
                        .Contains(s.LecturerId))
                    .ToArray();

                foreach (LecturerModule removedLecturer in toRemove)
                {
                    this.Context.LecturerModules.Remove(removedLecturer);

                    this.Context.Entry(removedLecturer).State = EntityState.Deleted;
                }
            }

            if (module.Semester.Course.Id != viewModel.CourseId)
            {
                Course course = this.Context.Courses.SingleOrDefault(s => s.Id == viewModel.CourseId);
                module.Semester.Course = course;
            }
            

            module.Name = viewModel.Name;
            module.ModuleType = viewModel.ModuleType;
            module.Credits = viewModel.Credits;

            this.Context.Entry(module).State = EntityState.Modified;

            try
            {
                Context.SaveChanges();

                return Json(module, this.Settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
            }
        }

        // POST: api/Modules
        [HttpPost]
        public IActionResult PostModule([FromBody] ModuleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            this.Context.LecturerModules.RemoveRange(this.Context.LecturerModules);
            this.Context.SaveChanges();

            Semester semester = this.Context.Semesters.Include(s => s.Course).SingleOrDefault(s => s.Id == viewModel.SemesterId);

            IEnumerable<LecturerPerson> lecturers = this.Context.LecturerPersons
                .Include(s => s.Lecturer)
                .Include(s => s.Person)
                .Where(s => viewModel.LecturerIds.Contains(s.LecturerId));

            Module module = new Module
            {
                Credits = viewModel.Credits,
                Name = viewModel.Name,
                Semester = semester,
                ModuleType = viewModel.ModuleType,
            };

            IEnumerable<LecturerModule> lecturerModules = lecturers.Select(s => new LecturerModule
            {
                LecturerId = s.LecturerId,
                Lecturer = s.Lecturer,
                Module = module,
                ModuleId = module.Id
            });

            module.LecturerModules = lecturerModules.ToArray();
            
            try
            {
                this.Context.Modules.Add(module);
                this.Context.SaveChanges();

                Response.StatusCode = StatusCodes.Status201Created;

                return Json(module, this.Settings);
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public IActionResult DeleteModule(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Module module = this.Context.Modules.Include(s => s.LecturerModules).Single(m => m.Id == id);
            if (module == null)
            {
                return HttpNotFound();
            }

            if (module.LecturerModules.Any())
            {
                this.Context.LecturerModules.RemoveRange(module.LecturerModules);
            }

            this.Context.Modules.Remove(module);
            this.Context.SaveChanges();

            return Ok(module);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ModuleExists(int id)
        {
            return Context.Modules.Count(e => e.Id == id) > 0;
        }
    }
}