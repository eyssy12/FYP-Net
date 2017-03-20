namespace CMS.Dashboard.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;
    using ViewModels.Configuration;

    [Authorize(Roles = "Admin,Student")]
    [Produces("application/json")]
    [Route("api/Persons")]
    public class PersonsController : Controller
    {
        private readonly Entities Context;
        private readonly MyJsonSerializerSettings Settings;
        private readonly UserManager<ApplicationUser> UserManager;

        public PersonsController(UserManager<ApplicationUser> userManager, Entities context, MyJsonSerializerSettings settings)
        {
            this.UserManager = userManager;
            this.Context = context;
            this.Settings = settings;
        }

        [Route("AvailablePersons")]
        public async Task<IActionResult> GetAvailablePersons()
        {
            IEnumerable<StudentPerson> studentPersons = this.Context.StudentPersons.Include(s => s.Person);
            IEnumerable<LecturerPerson> lecturerPersons = this.Context.LecturerPersons.Include(s => s.Person);

            IEnumerable<Person> persons = this.Context.Persons
                .Include(s => s.ApplicationUser)
                .ToArray();

            IList<Person> adminExclusions = new List<Person>();
            foreach (Person person in persons.Where(s => s.ApplicationUser != null)) // some students/ lecturers may not have had accounts assgined yet
            {
                bool isInRole = await this.UserManager.IsInRoleAsync(person.ApplicationUser, Enumerations.UserRole.Admin.ToString());

                if (isInRole)
                {
                    adminExclusions.Add(person);
                }
            }
            
            // filter out the ones that dont have either a lecturer or student assigned to the person type
            persons = persons.Except(studentPersons.Select(s => s.Person).Concat(lecturerPersons.Select(l => l.Person)));

            // now we also need to filter out the admins, they would never be students/lecturers thus should not be allowed for configuration
            persons = persons.Except(adminExclusions);

            return Json(persons, this.Settings);

        }

        [Route("AccountlessPersons")]
        public IActionResult GetAccountlessPersons()
        {
            IEnumerable<StudentPerson> studentPersons = this.Context.StudentPersons.Include(s => s.Person);
            IEnumerable<LecturerPerson> lecturerPersons = this.Context.LecturerPersons.Include(s => s.Person);

            IEnumerable<Person> persons = this.Context.Persons
                .Include(s => s.ApplicationUser)
                .ToArray();

            persons = persons.Where(s => s.ApplicationUser == null);

            return Json(persons, this.Settings);
        }

        // GET: api/Persons
        [HttpGet]
        public IActionResult GetPersons()
        {
            IEnumerable<Person> persons = this.Context.Persons.Include(s => s.MobileClients);

            return Json(persons, this.Settings);
        }

        // GET: api/Persons/5
        [HttpGet("{id}", Name = "GetPerson")]
        public IActionResult GetPerson([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Person person = Context.Persons.Single(m => m.Id == id);

            if (person == null)
            {
                return HttpNotFound();
            }

            return Ok(person);
        }

        // PUT: api/Persons/5
        [HttpPut("{id}")]
        public IActionResult PutPerson(int id, [FromBody] PersonViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            Person person = this.Context.Persons.SingleOrDefault(p => p.Id == viewModel.Id);

            person.FirstName = viewModel.FirstName;
            person.LastName = viewModel.LastName;
            person.MobilePhone = viewModel.MobilePhone;
            person.Address = viewModel.Address;
            person.BirthDate = viewModel.BirthDate;

            try
            {
                Context.Entry(person).State = EntityState.Modified;
                Context.SaveChanges();

                return Ok();
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        // POST: api/Persons
        [HttpPost]
        public IActionResult PostPerson([FromBody] PersonViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Person person = new Person
            {
                Address = viewModel.Address,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                BirthDate = viewModel.BirthDate,
                MobilePhone = viewModel.MobilePhone
            };

            try
            {
                Context.Persons.Add(person);
                Context.SaveChanges();

                return Json(person, this.Settings);
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        // DELETE: api/Persons/5
        [HttpDelete("{id}")]
        public IActionResult DeletePerson(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Person person = Context.Persons.Single(m => m.Id == id);
            if (person == null)
            {
                return HttpNotFound();
            }

            Context.Persons.Remove(person);
            Context.SaveChanges();

            return Ok(person);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PersonExists(int id)
        {
            return Context.Persons.Count(e => e.Id == id) > 0;
        }
    }
}