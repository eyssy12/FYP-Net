namespace CMS.Dashboard.Test.Controllers.Api
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
    using ViewModels;

    [Produces("application/json")]
    [Route("api/News")]
    public class NewsController : Controller
    {
        private readonly Entities Context;
        private readonly MyJsonSerializerSettings Settings;

        public NewsController(Entities context, MyJsonSerializerSettings settings)
        {
            this.Context = context;
            this.Settings = settings;
        }

        // GET: api/values
        [Authorize(Roles = "Lecturer,Admin,Student")]
        [HttpGet]
        public IActionResult GetNews()
        {
            IEnumerable<NewsPost> newsPosts = this.Context.NewsPosts
                .Include(s => s.ApplicationUser)
                    .ThenInclude(s => s.Person)
                .ToArray();

            return Json(newsPosts.Select(s => new
            {
                Id = s.Id,
                Title = s.Title,
                Body = s.Body,
                Timestamp = s.Timestamp,
                PostedBy = s.PostedBy,
                UserId = s.ApplicationUser == null ? null : s.ApplicationUser.Id,
                Person = s.ApplicationUser == null ? null : s.ApplicationUser.Person
            }), 
            this.Settings);
        }

        // GET api/values/5
        [Authorize(Roles = "Lecturer,Admin,Student")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [Authorize(Roles = "Lecturer,Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] NewsPostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            if (this.ValidateNewsPost(viewModel.Title, viewModel.Body, viewModel.ApplicationUserId))
            {
                ApplicationUser user = this.Context.Users.SingleOrDefault(s => s.Id == viewModel.ApplicationUserId);

                if (user != null)
                {
                    NewsPost post = new NewsPost
                    {
                        Title = viewModel.Title,
                        Body = viewModel.Body,
                        Timestamp = viewModel.Timestamp,
                        PostedBy = user.UserName,
                        ApplicationUser = user
                    };

                    try
                    {
                        Context.NewsPosts.Add(post);
                        Context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        return HttpBadRequest(ModelState);
                    }

                    Response.StatusCode = StatusCodes.Status201Created;

                    return Json(new
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Body = post.Body,
                        Timestamp = post.Timestamp,
                        PostedBy = user.UserName,
                        UserId = user.Id,
                        Person = user.Person
                    }, this.Settings);
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
        }

        // DELETE api/values/5
        [Authorize(Roles = "Lecturer,Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            NewsPost newsPost = this.Context.NewsPosts.Single(m => m.Id == id);
            if (newsPost == null)
            {
                return HttpNotFound();
            }
            
            this.Context.NewsPosts.Remove(newsPost);
            this.Context.SaveChanges();

            return Ok();
        }

        private bool ValidateNewsPost(params string[] properties)
        {
            foreach (string property in properties)
            {
                if (string.IsNullOrWhiteSpace(property))
                {
                    return false;
                }
            }

            return true;
        }
    }
}