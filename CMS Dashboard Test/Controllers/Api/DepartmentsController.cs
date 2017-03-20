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
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PushSharp.Google;
    using Serialization;
    using Services;
    using ViewModels;
    using ViewModels.Configuration;

    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/Departments")]
    [RequireHttps]
    public class DepartmentsController : Controller
    {
        private readonly Entities Context;
        private readonly JsonSerializerSettings Settings;
        private readonly INotifyService<GcmNotification> MobileClientNotifier;
        private readonly ILogger Logger;

        public DepartmentsController(Entities context, MyJsonSerializerSettings settings, INotifyService<GcmNotification> mobileClientNotifier, ILoggerFactory loggerFactory)
        {
            this.Context = context;
            this.Settings = settings;
            this.MobileClientNotifier = mobileClientNotifier;
            this.Logger = loggerFactory.CreateLogger<AccountController>();
            this.MobileClientNotifier.Start();
        }

        // GET: api/Departments
        [HttpGet]
        public IActionResult GetDepartments()
        {
            return Json(this.Context.Departments.Include(d => d.Courses), this.Settings);
        }

        [Route("withoutComplexTypes")]
        [HttpGet]
        public IActionResult GetDepartmentsWithoutComplexTypes()
        {
            return Json(this.Context.Departments, this.Settings);
        }

        // GET: api/Departments/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public IActionResult GetDepartment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Department department = Context.Departments.Include(d => d.Courses).Single(m => m.Id == id);

            if (department == null)
            {
                return HttpNotFound();
            }

            return Ok(department);
        }

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        public IActionResult PutDepartment(int id, [FromBody] DepartmentViewModel departmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != departmentViewModel.Id)
            {
                return HttpBadRequest();
            }

            Department department = this.Context.Departments.FirstOrDefault(d => d.Id == id);

            if (department.Name != departmentViewModel.Name)
            {
                department.Name = departmentViewModel.Name;

                this.Context.Entry(department).State = EntityState.Modified;

                try
                {
                    this.Context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.DepartmentExists(id))
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok();
            }

            return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
        }

        // POST: api/Departments
        [HttpPost]
        public IActionResult PostDepartment([FromBody] DepartmentViewModel departmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (!Context.Departments.Any(d => d.Name == departmentViewModel.Name))
            {
                Department department = new Department { Name = departmentViewModel.Name };
                Context.Departments.Add(department); // TODO: add courses - extensions for model to viewmodel too

                try
                {
                    Context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    this.Logger.LogError(ex.InnerException.Message);

                    return HttpBadRequest(ModelState);
                }

                Response.StatusCode = StatusCodes.Status201Created;

                return Json(department, this.Settings);
            }

            return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            Department department = Context.Departments.SingleOrDefault(m => m.Id == id);
            if (department == null)
            {
                return HttpNotFound();
            }

            Context.Departments.Remove(department);
            Context.SaveChanges();

            return Ok(department);
        }

        [Route("bulkDelete")]
        [HttpPost]
        public IActionResult BulkDelete([FromBody] IdArrayViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            IEnumerable<Department> departments = this.Context.Departments.Where(d => viewModel.Ids.Any(i => i == d.Id)).ToArray();

            if (departments.Any())
            {
                this.Context.Departments.RemoveRange(departments);
                this.Context.SaveChanges();

                return Ok();
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepartmentExists(int id)
        {
            return Context.Departments.Count(e => e.Id == id) > 0;
        }
    }
}