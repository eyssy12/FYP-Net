namespace CMS.Dashboard.Test.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Managers;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;
    using Services;
    using ViewModels.Configuration;
    using UserRole = CMS.Web.Configuration.Data.Enumerations.UserRole;

    [Produces("application/json")]
    [Route("api/ApplicationUsers")]
    public class ApplicationUsersController : Controller
    {
        private readonly Entities Context;
        private readonly MyJsonSerializerSettings Settings;
        private readonly UserManager<ApplicationUser> UserManager;

        public ApplicationUsersController(UserManager<ApplicationUser> userManager, Entities context, MyJsonSerializerSettings settings)
        {
            this.UserManager = userManager;
            this.Context = context;
            this.Settings = settings;
        }

        [HttpGet]
        [Route("loggedInUser")]
        [Authorize(Roles = "Student,Admin,Lecturer")]
        public IActionResult GetLoggedInUser()
        {
            IIdentity identity = this.User.Identity;
           
            string name = identity.Name;

            ApplicationUser user = this.Context.Users
                .Include(u => u.Person)
                .Include(u => u.Roles)
                .SingleOrDefault(s => s.UserName == name);

            IEnumerable<IdentityRole> roles = this.Context.Roles.ToArray();

            return Json(new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Person = user.Person,
                    UserRole = user.GetRoleFromUser(roles)
                }, this.Settings);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetApplicationUsers()
        {
            IEnumerable<ApplicationUser> users = this.Context.Users.Include(u => u.Roles).Include(u => u.Person).ToArray();
            IEnumerable<IdentityRole> roles = this.Context.Roles.ToArray();

            return Json(users.Select(u => new
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Person = u.Person,
                    UserRole = u.GetRoleFromUser(roles)
                }),
                this.Settings);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostApplicationUser([FromBody] ApplicationUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Person person = this.Context.Persons.Include(p => p.ApplicationUser).SingleOrDefault(s => s.Id == viewModel.PersonId);

            ApplicationUser user = new ApplicationUser
            {
                Email = viewModel.Email,
                UserName = viewModel.UserName,
                Person = person
            };

            try
            {
                IdentityResult createUserResult = await this.UserManager.CreateAsync(
                    user, 
                    string.IsNullOrWhiteSpace(viewModel.Password)
                        ? RandomPasswordGeneratorService.Generate() 
                        : viewModel.Password);

                IdentityResult addToRoleResult = await this.UserManager.AddToRoleAsync(user, viewModel.UserRole.ToString());

                if (createUserResult.Succeeded && addToRoleResult.Succeeded)
                {
                    Response.StatusCode = StatusCodes.Status201Created;

                    return Json(new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        UserRole = user.GetRoleFromUser(this.Context.Roles.ToArray())
                    }, this.Settings);
                }

                return HttpBadRequest();
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser(string id, [FromBody] ApplicationUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            ApplicationUser user = this.Context.Users.Include(u => u.Roles).SingleOrDefault(u => u.Id == viewModel.Id);

            if (user.Person == null || user.Person.Id != viewModel.PersonId)
            {
                Person person = this.Context.Persons.SingleOrDefault(p => p.Id == viewModel.PersonId);

                user.Person = person;
            }

            user.UserName = viewModel.UserName;
            user.Email = viewModel.Email;

            try
            {
                bool resulttest = await this.UserManager.IsInRoleAsync(user, UserRole.Admin.ToString());

                foreach (string role in Enum.GetNames(typeof(UserRole)))
                {
                    IdentityResult test1 = await this.UserManager.RemoveFromRoleAsync(user, role);
                }

                IdentityResult test2 = await this.UserManager.AddToRoleAsync(user, viewModel.UserRole.ToString());
                IdentityResult test3 = await this.UserManager.UpdateAsync(user);

                if (!string.IsNullOrWhiteSpace(viewModel.Password))
                {
                    // check if this works
                    var code = await this.UserManager.GeneratePasswordResetTokenAsync(user);
                    IdentityResult test4 = await UserManager.ResetPasswordAsync(user, code, viewModel.Password);
                }

                return Ok();
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteApplicationUser(string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            ApplicationUser user = Context.Users.SingleOrDefault(m => m.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            Context.Users.Remove(user);
            Context.SaveChanges();

            return Ok(user);
        }
    }
}