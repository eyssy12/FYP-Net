namespace TestApi.Controllers
{
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using Library.Bases;
    using Microsoft.AspNet.Mvc;
    using TestApi.Serialization;

    [Route(DepartmentController.DefaultApiControllerRoute)]
    public class DepartmentController : SecureControllerBase
    {
        public DepartmentController(Entities context, MyJsonSerializerSettings settings)
            : base(context, settings)
        {
        }

        // GET: api/department
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(this.Context.Departments.ToArray(), this.Settings);
        }
    }
}