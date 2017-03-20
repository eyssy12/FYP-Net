namespace CMS.Dashboard.Test.Controllers.Web
{
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;

    [Authorize(Roles = "Admin")]
    public class ConfigurationController : Controller
    {
        public IActionResult Department()
        {
            return View();
        }

        public IActionResult Course()
        {
            return View();
        }

        public IActionResult Semester()
        {
            return View();
        }

        public IActionResult Class()
        {
            return View();
        }

        public IActionResult Module()
        {
            return View();
        }

        public IActionResult Student()
        {
            return View();
        }

        public IActionResult Lecturer()
        {
            return View();
        }

        public IActionResult Person()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }

        public IActionResult Timetables()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
