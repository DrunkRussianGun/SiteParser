using Microsoft.AspNetCore.Mvc;

namespace SiteParser.Controllers
{
    public class ViewController : Controller
    {
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }
    }
}
