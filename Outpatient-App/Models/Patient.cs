namespace Outpatient_App.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string HealthCardID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Postcode { get; set; }
    }

}
