﻿using Microsoft.AspNetCore.Mvc;
using Outpatient_App.Models;
using System.Diagnostics;

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


        public IActionResult Disclaimer()
        {
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