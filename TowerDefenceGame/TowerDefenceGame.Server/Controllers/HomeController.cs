using Microsoft.AspNetCore.Mvc;

namespace TowerDefenceGame.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
