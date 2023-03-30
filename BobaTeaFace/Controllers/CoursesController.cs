using Microsoft.AspNetCore.Mvc;

namespace BobaTeaFace.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Course1()
        {
            return View();
        }
    }
}
