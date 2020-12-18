using Microsoft.AspNetCore.Mvc;

namespace SalesWebMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
