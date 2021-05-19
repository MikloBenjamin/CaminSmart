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

        public IActionResult Camine()
		{
            return RedirectToAction("Index", "Camine");
        }

        public IActionResult Camere()
        {
            return RedirectToAction("Index", "Camere");
        }
        public IActionResult Tichet()
        {
            return RedirectToAction("Index", "Tichet");
        }
        public IActionResult Student()
        {
            return RedirectToAction("Index", "Student");
        }
        public IActionResult Administratori()
        {
            return RedirectToAction("Index", "Administratori");
        }
        public IActionResult Inscriere()
        {
            return RedirectToAction("Index", "Applicant");
        }
        public IActionResult Tichete()
        {
            return RedirectToAction("Tichete", "Tichet");
        }

        public IActionResult CamineST()
        {
            return RedirectToAction("Camine", "Camine");
        }
        public IActionResult Applicants()
        {
            return RedirectToAction("Applicants", "Applicant");
        }
        public IActionResult Status()
        {
            return RedirectToAction("Status", "Student");
        }
        public IActionResult Index()
        {
            return RedirectToAction("Home", "Student");
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
