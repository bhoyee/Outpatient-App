using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Outpatient_App.Data;
using Outpatient_App.updateHubs;
using Outpatient_App.Models;
using System.Globalization;

namespace Outpatient_App.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming you're using Entity Framework Core for data access
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly ILogger<AppointmentController> _logger;


        public AppointmentController(ApplicationDbContext context, IHubContext<UpdateHub> hubContext, ILogger<AppointmentController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        public IActionResult BookAppointment()
        {
            // Display a form for booking appointments
            return View();
        }

        [HttpPost]
        public IActionResult BookNow(string dateOfBirth, string surname, string postcode)
        {
            try
            {
                Console.WriteLine($"Received request with dateOfBirth: {dateOfBirth}, surname: {surname}, postcode: {postcode}");

                DateTime formattedDateOfBirth;

                // Try parsing with multiple date formats
                string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy" };
                if (!DateTime.TryParseExact(dateOfBirth.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDateOfBirth))
                {
                    Console.WriteLine($"Failed to parse dateOfBirth: {dateOfBirth}");
                    throw new FormatException("Invalid date format. Please enter a valid date in the 'dd/MM/yyyy' format.");
                }

                var trimmedSurname = surname.Trim();
                var trimmedPostcode = postcode.Trim();
                var formattedDateForQuery = formattedDateOfBirth.ToString("yyyy-MM-dd");
                Console.WriteLine($"Formatted date for query: {formattedDateForQuery}");

                // Check if the patient has an appointment for the current day
                var existingAppointment = _context.Appointments
                    .Where(a => a.PatientID != null)  // Assuming PatientID is not nullable
                    .Join(
                        _context.Patients,
                        app => app.PatientID,
                        pat => pat.PatientID,
                        (app, pat) => new { Appointment = app, Patient = pat }
                    )
                    .Where(joined => joined.Patient.Surname.StartsWith(trimmedSurname) &&
                                     joined.Patient.DateOfBirth.Date == formattedDateOfBirth.Date.AddDays(1) &&
                                     joined.Patient.Postcode == trimmedPostcode)
                    .FirstOrDefault();

                if (existingAppointment != null)
                {
                    // Duplicate appointment found
                    Console.WriteLine($"Duplicate appointment found. Patient ID: {existingAppointment.Appointment.PatientID}");

                    // You can add code here to handle the display of the duplicate appointment message
                    return Json(new { success = false, message = "Duplicate appointment found. You already have an appointment booked for today." });
                }

                // If no duplicate appointment is found, proceed with booking
                var query = @"
            SELECT TOP 1 *
            FROM Patients
            WHERE Surname LIKE @Surname
              AND CAST(DateOfBirth AS DATE) = @DateOfBirth
              AND Postcode = @Postcode
        ";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@Surname", $"{trimmedSurname}%"),
            new SqlParameter("@DateOfBirth", formattedDateForQuery),
            new SqlParameter("@Postcode", trimmedPostcode)
                };

                var patient = _context.Patients
                    .FromSqlRaw(query, parameters)
                    .FirstOrDefault();

                if (patient != null)
                {
                    // Perform additional operations, check for appointments, etc.

                    // Example: Creating an object and adding it to the database
                    var appointment = new Appointment
                    {
                        PatientID = patient.PatientID,
                        ScheduledTime = DateTime.Now.Date.AddDays(1), // Set time to next day of current date
                        CheckedIn = false
                        // Add any other properties for the appointment here
                    };

                    _context.Appointments.Add(appointment);
                    _context.SaveChanges();

                    return Json(new { success = true, patientId = patient.PatientID, appointmentId = appointment.AppointmentID });
                }
                else
                {
                    // Log the details for debugging
                    Console.WriteLine($"Patient not found. Details: Date of Birth - {formattedDateOfBirth}, Surname - {trimmedSurname}, Postcode - {trimmedPostcode}");

                    return Json(new { success = false, message = "Patient not found. Please provide valid details." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception: {ex}");

                if (ex is FormatException)
                {
                    // Handle the format exception with a specific message
                    return Json(new { success = false, message = "Invalid date format. Please enter a valid date in the 'dd/MM/yyyy' format." });
                }
                else
                {
                    // Handle other exceptions with a generic message
                    return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
                }
            }
        }

        public async Task<IActionResult> AppointmentsTodayDetails()
        {
            try
            {
                // Get today's date
                DateTime today = DateTime.Today;

                // Fetch the required information from the database
                var appointmentDetails = await _context.Appointments
                    .Where(appointment => EF.Functions.DateDiffDay(appointment.ScheduledTime.Date, today) == 0)
                    .Join(
                        _context.Patients,
                        appointment => appointment.PatientID,
                        patient => patient.PatientID,
                        (appointment, patient) => new
                        {
                            patient.FirstName,
                            patient.Surname,
                            patient.DateOfBirth,
                            patient.Postcode,
                            appointment.CheckedIn
                        }
                    )
                    .ToListAsync();

                // Log raw data for debugging
                _logger.LogInformation("Raw appointmentDetails: {appointmentDetails}", appointmentDetails);

                // Combine first name and surname, and format date of birth
                var formattedDetails = appointmentDetails.Select(detail => new
                {
                    FullName = $"{detail.FirstName} {detail.Surname}",
                    DateOfBirth = detail.DateOfBirth.ToString("yyyy-MM-dd"), // Format the date
                    detail.Postcode,
                    detail.CheckedIn
                });

                // Log formatted data for debugging
                _logger.LogInformation("Formatted details: {formattedDetails}", formattedDetails);

                // Return the data as JSON
                return Json(formattedDetails);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response if needed
                _logger.LogError($"Exception in AppointmentsTodayDetails: {ex}");
                return StatusCode(500, "An error occurred while fetching appointment details.");
            }
        }







    }

}
