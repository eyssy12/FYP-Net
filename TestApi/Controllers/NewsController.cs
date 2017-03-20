namespace TestApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Library.Bases;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Serialization;

    [Route(NewsController.DefaultApiControllerRoute)]
    public class NewsController : SecureControllerBase
    {
        public NewsController(Entities context, MyJsonSerializerSettings settings)
            : base(context, settings)
        {
        }

        // GET: api/news
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<NewsPost> newsPosts = this.Context.NewsPosts
                .Include(s => s.ApplicationUser)
                    .ThenInclude(s => s.Person)
                .ToArray();

            return Json(newsPosts.Select(s => new
            {
                Title = s.Title,
                Body = s.Body,
                Timestamp = s.Timestamp,
                PostedBy = s.PostedBy,
                UserId = s.ApplicationUser == null ? null : s.ApplicationUser.Id,
                Person = s.ApplicationUser == null ? null : s.ApplicationUser.Person
            }),
            this.Settings);
        }
    }
}