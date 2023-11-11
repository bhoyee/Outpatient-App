using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outpatient_App.Models;
using System.Globalization;

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

        //[HttpPost]
        //public IActionResult BookAppointment(string surname, DateTime? dateOfBirth)
        //{
        //    if (!string.IsNullOrEmpty(surname) && dateOfBirth != null)
        //    {
        //        var patient = _context.Patients.FirstOrDefault(p =>
        //            p.Surname.StartsWith(surname) && p.DateOfBirth == dateOfBirth);

        //        if (patient != null)
        //        {
        //            var appointment = new Appointment();
        //            appointment.PatientID = patient.PatientID;
        //            _context.Appointments.Add(appointment);
        //            _context.SaveChanges();
        //            return RedirectToAction(nameof(Confirmation), new { appointmentId = appointment.AppointmentID });
        //        }
        //    }

        //    ModelState.AddModelError("", "Patient not found. Please provide valid details.");
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult BookAppointment(string surname, DateTime? dateOfBirth, string postcode)
        //{
        //    if (!string.IsNullOrEmpty(surname) && dateOfBirth != null && !string.IsNullOrEmpty(postcode))
        //    {
        //        var patient = _context.Patients.FirstOrDefault(p =>
        //            p.Surname.StartsWith(surname) && p.DateOfBirth == dateOfBirth && p.Postcode == postcode);

        //        if (patient != null)
        //        {
        //            var appointment = new Appointment
        //            {
        //                PatientID = patient.PatientID,
        //                ScheduledTime = DateTime.Now.AddDays(1) // Book appointment for the next day
        //            };

        //            _context.Appointments.Add(appointment);
        //            _context.SaveChanges();

        //            return RedirectToAction(nameof(Confirmation), new { appointmentId = appointment.AppointmentID });
        //        }
        //        else
        //        {
        //            return RedirectToAction(nameof(UserNotFound));
        //        }
        //    }

        //    ModelState.AddModelError("", "Invalid details provided.");
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult BookNow(string dateOfBirth, string surname, string postcode)
        //{
        //    // Hardcoded values for testing
        //    var formattedDateOfBirth = "29/01/1988";
        //    var trimmedSurname = "S";
        //    var trimmedPostcode = "HU51PQ";

        //    var patient = _context.Patients.FirstOrDefault(p =>
        //        p.Surname.Equals(trimmedSurname, StringComparison.OrdinalIgnoreCase) &&
        //        EF.Functions.Like(p.DateOfBirth.ToString(), formattedDateOfBirth) &&
        //        p.Postcode.Equals(trimmedPostcode, StringComparison.OrdinalIgnoreCase));

        //    if (patient != null)
        //    {
        //        // Patient found, return JSON indicating success
        //        return Json(new { success = true, patientId = patient.PatientID });
        //    }
        //    else
        //    {
        //        // Patient not found, return JSON indicating failure
        //        return Json(new { success = false, message = "Patient not found. Please provide valid details." });
        ////    }
        //}

        [HttpPost]
        public IActionResult BookNow(string dateOfBirth, string surname, string postcode)
        {
            var formattedDateOfBirth = dateOfBirth.Trim();
            var trimmedSurname = surname.Trim();
            var trimmedPostcode = postcode.Trim();

            var query = $@"
        SELECT TOP 1 *
        FROM Patients
        WHERE Surname LIKE '{trimmedSurname}%' 
          AND CONVERT(DATE, DateOfBirth, 103) = '{formattedDateOfBirth}'
          AND Postcode = '{trimmedPostcode}'
    ";

            var patient = _context.Patients.FromSqlRaw(query).FirstOrDefault();

            if (patient != null)
            {
                // Check if the patient already has an appointment on the specified date
                var existingAppointment = _context.Appointments
                    .FirstOrDefault(a => a.PatientID == patient.PatientID &&
                                         a.ScheduledTime.Date == DateTime.Now.Date.AddDays(1));

                if (existingAppointment != null)
                {
                    // Patient already has an appointment on the specified date
                    return Json(new { success = false, message = "Patient already has an appointment on the specified date." });
                }

                // Patient found, insert into Appointment table
                var appointment = new Appointment
                {
                    PatientID = patient.PatientID,
                    ScheduledTime = DateTime.Now.Date.AddDays(1), // Set time to midnight
                    CheckedIn = false
                    // Add any other properties for the appointment here
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                // Return JSON indicating success
                return Json(new { success = true, patientId = patient.PatientID, appointmentId = appointment.AppointmentID });
            }
            else
            {
                // Patient not found, return JSON indicating failure
                return Json(new { success = false, message = "Patient not found. Please provide valid details." });
            }
        }


        public IActionResult Confirmation(int appointmentId)
        {
            // Retrieve appointment details if needed
            return View();
        }

        public IActionResult UserNotFound()
        {
            return View();
        }





        //public IActionResult CheckIn()
        //{
        //    // Display a form for checking in appointments
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult CheckIn(Appointment appointment)
        //{
        //    var existingAppointment = _context.Appointments.FirstOrDefault(a => a.AppointmentID == appointment.AppointmentID);

        //    if (existingAppointment != null)
        //    {
        //        existingAppointment.CheckedIn = true;
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Confirmation), new { appointmentId = existingAppointment.AppointmentID });
        //    }

        //    return View(appointment);
        //}

        //public IActionResult Confirmation(int appointmentId)
        //{
        //    var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
        //    if (appointment != null)
        //    {
        //        return View(appointment);
        //    }
        //    return NotFound();
        //}
    }

}
