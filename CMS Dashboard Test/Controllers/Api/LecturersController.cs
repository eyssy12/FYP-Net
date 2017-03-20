namespace CMS.Dashboard.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Dashboard.Test.Serialization;
    using CMS.Dashboard.Test.ViewModels.Configuration;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;

    [Authorize(Roles ="Student,Admin,Lecturer")]
    [Produces("application/json")]
    [Route("api/Lecturers")]
    public class LecturersController : Controller
    {
        private readonly Entities Context;
        private readonly MyJsonSerializerSettings Settings;

        public LecturersController(Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Settings = settings;
        }

        // GET: api/Lecturers
        [HttpGet]
        public IActionResult GetLecturers()
        {
            IEnumerable<Lecturer> lecturers = this.Context.Lecturers
                .Include(l => l.LecturerPerson).ThenInclude(lp => lp.Person)
                .Include(l => l.LecturerModules)
                .ToArray();

            return Json(lecturers, this.Settings);
        }

        // GET: api/Lecturers/5
        [HttpGet("{id}", Name = "GetLecturer")]
        public IActionResult GetLecturer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Lecturer lecturer = Context.Lecturers.Single(m => m.Id == id);

            if (lecturer == null)
            {
                return HttpNotFound();
            }

            return Ok(lecturer);
        }

        // PUT: api/Lecturers/5
        [HttpPut("{id}")]
        public IActionResult PutLecturer(int id, [FromBody] Lecturer lecturer)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != lecturer.Id)
            {
                return HttpBadRequest();
            }

            Context.Entry(lecturer).State = EntityState.Modified;

            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LecturerExists(id))
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

        // POST: api/Lecturers
        [HttpPost]
        public IActionResult PostLecturer([FromBody] LecturerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Person person = this.Context.Persons.Include(p => p.MobileClients).SingleOrDefault(p => p.Id == viewModel.PersonId);

            Lecturer lecturer = new Lecturer
            {
                HireDate = viewModel.HireDate
            };

            LecturerPerson lecturerPerson = new LecturerPerson
            {
                Person = person,
                PersonId = person.Id,
                LecturerId = lecturer.Id
            };

            lecturer.LecturerPerson = lecturerPerson;

            try
            {
                Context.Lecturers.Add(lecturer);
                Context.SaveChanges();

                return Json(lecturer, this.Settings);
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        // DELETE: api/Lecturers/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLecturer(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Lecturer lecturer = this.Context.Lecturers
                .Include(s => s.LecturerPerson)
                .Include(s => s.LecturerModules)
                .Single(m => m.Id == id);

            if (lecturer == null)
            {
                return HttpNotFound();
            }

            try
            {
                this.Context.LecturerModules.RemoveRange(lecturer.LecturerModules);
                this.Context.Lecturers.Remove(lecturer);

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

        private bool LecturerExists(int id)
        {
            return Context.Lecturers.Count(e => e.Id == id) > 0;
        }
    }
}