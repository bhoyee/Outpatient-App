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
                _logger.LogInformation($"Received request with dateOfBirth: {dateOfBirth}, surname: {surname}, postcode: {postcode}");

                DateTime formattedDateOfBirth;

                // Try parsing with multiple date formats
                string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy" };
                if (!DateTime.TryParseExact(dateOfBirth.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDateOfBirth))
                {
                    _logger.LogError($"Failed to parse dateOfBirth: {dateOfBirth}");
                    throw new FormatException("Invalid date format. Please enter a valid date in the 'dd/MM/yyyy' format.");
                }

                var trimmedSurname = surname.Trim();
                var trimmedPostcode = postcode.Trim();
                var formattedDateOfBirthForQuery = formattedDateOfBirth.ToString("yyyy-MM-dd");
                _logger.LogInformation($"Formatted date for query: {formattedDateOfBirthForQuery}");


                string currentFormattedDate = DateTime.Now.ToString("dd/MM/yyyy");
                _logger.LogInformation($"CURRENT FORMATED DATE : {currentFormattedDate}");

                //var existingAppointment = _context.Appointments
                //    .Where(a => a.PatientID != null && a.ScheduledTime.Date != DateTime.Now.Date)
                //    .Join(
                //        _context.Patients,
                //        app => app.PatientID,
                //        pat => pat.PatientID,
                //        (app, pat) => new { Appointment = app, Patient = pat }
                //    )
                //    .Where(joined => joined.Patient.Surname.StartsWith(trimmedSurname) &&
                //                     joined.Patient.DateOfBirth.Date == formattedDateOfBirth.Date &&
                //                     joined.Patient.Postcode == trimmedPostcode)
                //    .FirstOrDefault();
                //var existingAppointment = _context.Appointments
                //    .Where(a => a.PatientID != null && EF.Functions.DateDiffDay(a.ScheduledTime.Date, DateTime.Now.Date) == 0)
                //    .Join(
                //        _context.Patients,
                //        app => app.PatientID,
                //        pat => pat.PatientID,
                //        (app, pat) => new { Appointment = app, Patient = pat }
                //    )
                //    .Where(joined => joined.Patient.Surname.StartsWith(trimmedSurname) &&
                //                     joined.Patient.DateOfBirth.Date == formattedDateOfBirth.Date &&
                //                     joined.Patient.Postcode == trimmedPostcode)
                //    .FirstOrDefault();
                var existingAppointmentsToday = _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.Patient.DateOfBirth.Date == formattedDateOfBirth.Date &&
                                a.Patient.Surname.StartsWith(trimmedSurname) &&
                                a.Patient.Postcode == trimmedPostcode &&
                                EF.Functions.DateDiffDay(a.ScheduledTime.Date, DateTime.Now.Date.AddDays(1)) == 0)
                    .ToList();


                if (existingAppointmentsToday.Any())
                {
                    // Duplicate appointment found
                  //  _logger.LogInformation($"Duplicate appointment found. Patient ID: {existingAppointmentsToday.Appointment.PatientID}, Shedule time: {existingAppointment.Appointment.ScheduledTime}");

                    // You can add code here to handle the display of the duplicate appointment message
                    return Json(new { success = false, message = "Duplicate appointment found. You already have an appointment booked for this date." });
                }
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
                    new SqlParameter("@DateOfBirth", formattedDateOfBirthForQuery),
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
                        ScheduledTime = DateTime.Now.Date.AddDays(1), // Set time to the next day of the current date
                        CheckedIn = false
                        // Add any other properties for the appointment here
                    };

                    _context.Appointments.Add(appointment);
                    _context.SaveChanges();

                    _logger.LogInformation($"Appointment booked successfully. Patient ID: {patient.PatientID}, Appointment ID: {appointment.AppointmentID}");

                    return Json(new { success = true, patientId = patient.PatientID, appointmentId = appointment.AppointmentID });
                }
                else
                {
                    // Log the details for debugging
                    _logger.LogInformation($"Patient not found. Details: Date of Birth - {formattedDateOfBirthForQuery}, Surname - {trimmedSurname}, Postcode - {trimmedPostcode}");

                    return Json(new { success = false, message = "Patient not found. Please provide valid details." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _logger.LogError($"Exception: {ex}");

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
