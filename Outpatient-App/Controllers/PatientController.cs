using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Import the ILogger namespace
using Outpatient_App.Data;
using Outpatient_App.Models;
using System;
using System.Linq;

namespace Outpatient_App.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientController> _logger; // Logger instance

        public PatientController(ApplicationDbContext context, ILogger<PatientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingPatient = _context.Patients.FirstOrDefault(p =>
                        p.FirstName == patient.FirstName &&
                        p.Surname == patient.Surname &&
                        p.DateOfBirth == patient.DateOfBirth &&
                        p.Postcode == patient.Postcode);

                    if (existingPatient != null)
                    {
                        ViewBag.Duplicate = "Patient with similar details already exists";
                        return View(patient);
                    }

                    _context.Patients.Add(patient);
                    _context.SaveChanges();

                    _logger.LogInformation("New patient added: {PatientId}", patient.PatientID); // Log successful addition
                    ViewBag.Success = "Patient added successfully";
                    ModelState.Clear(); // Clear ModelState to avoid repopulating old data in the form
                    return View(); // Return an empty form after successful addition

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding new patient."); // Log error if something goes wrong
                    ModelState.AddModelError(string.Empty, "Error adding new patient.");
                    ViewBag.Failure = "Error adding new patient";
                    return View(patient);
                }
            }

            return View(patient);
        }


        public IActionResult Success()
        {
            return View();
        }
    }
}
