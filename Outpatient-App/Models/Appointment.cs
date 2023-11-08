namespace Outpatient_App.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool CheckedIn { get; set; }
    }
}
