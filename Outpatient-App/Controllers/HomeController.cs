using Microsoft.AspNetCore.Mvc;
using Outpatient_App.Models;
using System.Diagnostics;
using System.Globalization;

namespace Outpatient_App.Controllers
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LanguageSelection()
        {
            return View();
        }


        //public IActionResult Disclaimer()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult Disclaimer(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                CultureInfo.CurrentCulture = new CultureInfo(lang);
                CultureInfo.CurrentUICulture = new CultureInfo(lang);
            }

            return View();
        }



        public IActionResult ContactReception()
        {
            return View();
        }
        public IActionResult Action()
        {
            return View();
        }
    }
}