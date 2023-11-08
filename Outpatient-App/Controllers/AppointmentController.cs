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
        public IActionResult BookAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                return RedirectToAction(nameof(Confirmation), new { appointmentId = appointment.AppointmentID });
            }
            return View(appointment);
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
