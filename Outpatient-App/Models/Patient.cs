namespace Outpatient_App.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string FullName { get; set; }
        public string HealthCardID { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
