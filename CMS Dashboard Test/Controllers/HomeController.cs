namespace CMS.Dashboard.Test.Controllers
{
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;

    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}