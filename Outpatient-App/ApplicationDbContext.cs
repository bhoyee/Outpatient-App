using Microsoft.EntityFrameworkCore;
using Outpatient_App.Models;
using System.Collections.Generic;

namespace Outpatient_App
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }

}
