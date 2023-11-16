using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Import the ILogger namespace
using Outpatient_App.Data;
using Outpatient_App.Models;
using System;
using System.Linq;

namespace Outpatient_App.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientsController> _logger; // Logger instance

        public PatientsController(ApplicationDbContext context, ILogger<PatientsController> logger)
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
                    var existingPatient = _context.Patients.FirstOrDefault(p => p.HealthCardID == patient.HealthCardID);
                    if (existingPatient != null)
                    {
                        ModelState.AddModelError("HealthCardID", "Health Card ID already exists");
                        return View(patient);
                    }

                    _context.Patients.Add(patient);
                    _context.SaveChanges();

                    _logger.LogInformation("New patient added: {PatientId}", patient.PatientID); // Log successful addition

                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding new patient."); // Log error if something goes wrong
                    ModelState.AddModelError(string.Empty, "Error adding new patient.");
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
