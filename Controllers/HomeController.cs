using AplicatieCamine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AplicatieCamine.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Camine()
		{
            return RedirectToAction("Index", "Camine");
        }

        public async Task<IActionResult> Camere()
        {
            return RedirectToAction("Index", "Camere");
        }
        public async Task<IActionResult> Tichet()
        {
            return RedirectToAction("Index", "Tichet");
        }
        public async Task<IActionResult> Student()
        {
            return RedirectToAction("Index", "Student");
        }
        public async Task<IActionResult> Administratori()
        {
            return RedirectToAction("Index", "Administratori");
        }
        public async Task<IActionResult> Inscriere()
        {
            return RedirectToAction("Inscriere", "Student");
        }
        public async Task<IActionResult> Tichete()
        {
            return RedirectToAction("Tichete", "Tichet");
        }
        public async Task<IActionResult> CamineST()
        {
            return RedirectToAction("Camine", "Camine");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
