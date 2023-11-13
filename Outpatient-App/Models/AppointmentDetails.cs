namespace Outpatient_App.Models
{
    public class AppointmentDetails
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Postcode { get; set; }
        public bool CheckedIn { get; set; }
    }
}
