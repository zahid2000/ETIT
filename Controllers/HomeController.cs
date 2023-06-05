using ETIT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ETIT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        } 

        public async Task<IActionResult> Detail(int id)
        {
            return View();
        }
        
    }
}