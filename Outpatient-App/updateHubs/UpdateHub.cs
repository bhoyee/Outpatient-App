using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Outpatient_App.updateHubs
{
    public class UpdateHub :Hub
    {
        private readonly ILogger<UpdateHub> _logger;

        public UpdateHub(ILogger<UpdateHub> logger)
        {
            _logger = logger;
        }

        public async Task UpdateTotalAppointments(int totalAppointmentsForToday)
        {
            _logger.LogInformation($"Received update from UpdateHub: {totalAppointmentsForToday}");

            await Clients.All.SendAsync("UpdateTotalAppointments", totalAppointmentsForToday);
        }
    }
}
