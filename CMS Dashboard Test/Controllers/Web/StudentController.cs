namespace CMS.Dashboard.Test.Controllers.Web
{
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Views.Models;

    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IActionResult Modules()
        {
            return View();
        }

        public IActionResult Classmates()
        {
            return View();
        }

        public IActionResult Timetables()
        {
            return View();
        }

        public IActionResult ModuleView(int id)
        {
            ModuleViewViewModel model = new ModuleViewViewModel();
            model.ModuleId = id;

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
