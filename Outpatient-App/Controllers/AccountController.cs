using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Outpatient_App.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // Import the ILogger interface
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Outpatient_App.Data;
using Outpatient_App.updateHubs;
using System.Text.Json;

namespace Outpatient_App.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming you're using Entity Framework Core for data access
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly ILogger<AppointmentController> _logger;

        public AccountController(ApplicationDbContext context, IHubContext<UpdateHub> hubContext, ILogger<AppointmentController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

            public IActionResult Index()
        {
            return View();
        }

       
        public async Task<IActionResult> DashboardAsync()
        {
            try
            {
                _logger.LogInformation("Dashboard action started.");

                // Add your code to fetch data from the database here
                var data = _context.Appointments.ToList(); // Replace YourDataEntity with your actual entity
                _logger.LogInformation("data");
                await UpdateAppointments();
                _logger.LogInformation("Dashboard action completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the Dashboard action.");
                // You might want to handle the exception or redirect to an error page
            }

            return View();
        }

        //public async Task<IActionResult> UpdateAppointments()
        //{
        //    try
        //    {
        //        // Log information for debugging
        //        _logger.LogInformation("Updating appointments...");

        //        // Calculate the updated count
        //        DateTime today = DateTime.Today;

        //        int totalAppointmentsForToday = await _context.Appointments
        //            .CountAsync(appointment => EF.Functions.DateDiffDay(appointment.ScheduledTime.Date, today) == 0);

        //        // Log the count for debugging
        //        _logger.LogInformation($"Total appointments for today: {totalAppointmentsForToday}");

        //        // Log success for debugging
        //        _logger.LogInformation("Appointments updated successfully.");

        //        // Return total appointments as JSON with lowercase property name
        //        return Json(new { totalAppointmentsForToday });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for debugging
        //        _logger.LogError($"Exception in UpdateAppointments: {ex}");
        //        return StatusCode(500, "An error occurred while updating appointments.");
        //    }
        //}

        //public async Task<IActionResult> UpdateAppointments()
        //{
        //    try
        //    {
        //        // Log information for debugging
        //        _logger.LogInformation("Updating appointments...");

        //        // Calculate the updated counts
        //        DateTime today = DateTime.Today;
        //        DateTime nextDay = today.AddDays(1);

        //        int totalAppointmentsForToday = await _context.Appointments
        //            .CountAsync(appointment => EF.Functions.DateDiffDay(appointment.ScheduledTime.Date, today) == 0);

        //        int totalAppointmentsForNextDay = await _context.Appointments
        //            .CountAsync(appointment => EF.Functions.DateDiffDay(appointment.ScheduledTime.Date, nextDay) == 0);

        //        int totalCheckedInAppointments = await _context.Appointments
        //            .CountAsync(appointment => appointment.CheckedIn == true);

        //        // Log the counts for debugging
        //        _logger.LogInformation($"Total appointments for today: {totalAppointmentsForToday}");
        //        _logger.LogInformation($"Total appointments for next day: {totalAppointmentsForNextDay}");
        //        _logger.LogInformation($"Total checked-in appointments: {totalCheckedInAppointments}");

        //        // Send the update to connected clients using SignalR
        //        await _hubContext.Clients.All.SendAsync("UpdateTotalAppointments", new
        //        {
        //            totalAppointmentsForToday,
        //            totalAppointmentsForNextDay,
        //            totalCheckedInAppointments
        //        });

        //        // Log success for debugging
        //        _logger.LogInformation("Appointments updated successfully.");

        //        // Return total appointments as JSON with lowercase property names
        //        return Json(new
        //        {
        //            totalAppointmentsForToday,
        //            totalAppointmentsForNextDay,
        //            totalCheckedInAppointments
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for debugging
        //        _logger.LogError($"Exception in UpdateAppointments: {ex}");
        //        return StatusCode(500, "An error occurred while updating appointments.");
        //    }
        //}

        public async Task<IActionResult> UpdateAppointments()
        {
            try
            {
                // Log information for debugging
                _logger.LogInformation("Updating appointments...");

                // Calculate the updated count
                DateTime today = DateTime.Today;

                int totalAppointmentsForToday = await _context.Appointments
                    .CountAsync(appointment => EF.Functions.DateDiffDay(appointment.ScheduledTime.Date, today) == 0);

                int totalAppointmentsForNextDay = await _context.Appointments
                    .CountAsync(appointment => EF.Functions.DateDiffDay(appointment.ScheduledTime.Date, today.AddDays(1)) == 0);

                int totalCheckedInAppointments = await _context.Appointments
                 .CountAsync(appointment => appointment.CheckedIn == true);

                // Get monthly data
                var monthlyData = await _context.Appointments
                    .Where(appointment => appointment.ScheduledTime.Month == today.Month)
                    .GroupBy(appointment => appointment.ScheduledTime.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Count = group.Count()
                    })
                    .OrderBy(entry => entry.Date)
                    .ToListAsync();

                // Extract labels and data for the area chart
                var monthlyLabels = monthlyData.Select(entry => entry.Date.Day.ToString()).ToList();
                var monthlyCounts = monthlyData.Select(entry => entry.Count).ToList();

                // Log the count for debugging
                _logger.LogInformation($"Total appointments for today: {totalAppointmentsForToday}");

                // Log success for debugging
                _logger.LogInformation("Appointments updated successfully.");

                // Return total appointments as JSON with lowercase property names
                return Json(new
                {
                    totalAppointmentsForToday,
                    totalAppointmentsForNextDay,
                    totalCheckedInAppointments,
                    monthlyData = new
                    {
                        labels = monthlyLabels,
                        data = monthlyCounts
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _logger.LogError($"Exception in UpdateAppointments: {ex}");
                return StatusCode(500, "An error occurred while updating appointments.");
            }
        }



    }
}
