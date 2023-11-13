using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Outpatient_App.Data;

namespace Outpatient_App.Controllers
{
    public class CheckInController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming you're using Entity Framework Core for data access

        public CheckInController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CheckIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckIn(string dateOfBirth, string surnameLetter)
        {
            var formattedDateOfBirth = dateOfBirth.Trim();
            var trimmedSurnameLetter = surnameLetter.Trim();

            var query = $@"
        SELECT TOP 1 A.*
        FROM Appointments A
        INNER JOIN Patients P ON A.PatientID = P.PatientID
        WHERE CONVERT(DATE, P.DateOfBirth, 103) = '{formattedDateOfBirth}'
            AND SUBSTRING(P.Surname, 1, 1) = '{trimmedSurnameLetter}'
            AND CONVERT(DATE, A.ScheduledTime) = CONVERT(DATE, GETDATE())
    ";

            var appointment = _context.Appointments.FromSqlRaw(query).FirstOrDefault();

            if (appointment != null)
            {
                if (appointment.CheckedIn)
                {
                    // Patient already checked in
                    return Json(new { success = true, alreadyCheckedIn = true, message = "Patient already checked in." });
                }

                // Update the CheckedIn status to true
                appointment.CheckedIn = true;
                _context.SaveChanges();

                // Return JSON indicating success without alreadyCheckedIn property
                return Json(new { success = true, message = "Patient checked in successfully." });
            }
            else
            {
                // Patient doesn't have an appointment today
                return Json(new { success = false, message = "No appointment today for the provided details." });
            }
        }
    }
}
