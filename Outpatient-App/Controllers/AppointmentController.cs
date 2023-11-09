using Microsoft.AspNetCore.Mvc;
using Outpatient_App.Models;

namespace Outpatient_App.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming you're using Entity Framework Core for data access

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult BookAppointment()
        {
            // Display a form for booking appointments
            return View();
        }

        [HttpPost]
        public IActionResult BookAppointment(string surname, DateTime? dateOfBirth)
        {
            if (!string.IsNullOrEmpty(surname) && dateOfBirth != null)
            {
                var patient = _context.Patients.FirstOrDefault(p =>
                    p.Surname.StartsWith(surname) && p.DateOfBirth == dateOfBirth);

                if (patient != null)
                {
                    var appointment = new Appointment();
                    appointment.PatientID = patient.PatientID;
                    _context.Appointments.Add(appointment);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Confirmation), new { appointmentId = appointment.AppointmentID });
                }
            }

            ModelState.AddModelError("", "Patient not found. Please provide valid details.");
            return View();
        }





        public IActionResult CheckIn()
        {
            // Display a form for checking in appointments
            return View();
        }

        [HttpPost]
        public IActionResult CheckIn(Appointment appointment)
        {
            var existingAppointment = _context.Appointments.FirstOrDefault(a => a.AppointmentID == appointment.AppointmentID);

            if (existingAppointment != null)
            {
                existingAppointment.CheckedIn = true;
                _context.SaveChanges();
                return RedirectToAction(nameof(Confirmation), new { appointmentId = existingAppointment.AppointmentID });
            }

            return View(appointment);
        }

        public IActionResult Confirmation(int appointmentId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
            if (appointment != null)
            {
                return View(appointment);
            }
            return NotFound();
        }
    }

}
