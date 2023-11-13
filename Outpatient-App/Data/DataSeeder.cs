//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using Outpatient_App.Models;
//using System;
//using System.Threading.Tasks;

//namespace Outpatient_App.Data
//{
//    public static class DataSeeder
//    {
//        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<Admin> userManager, ILogger logger)
//        {
//            var adminUser = await userManager.FindByNameAsync("admin");

//            if (adminUser == null)
//            {
//                await SeedAdminUser(userManager, "admin", "Password01!", logger); // Include a non-alphanumeric character
//                await SeedPatients(serviceProvider, logger); // Add this line to seed patients
//                                                             // Add more seed data if needed
//            }
//        }


//        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager, ILogger logger)
//        {
//            string[] roleNames = { "Admin" };

//            foreach (var roleName in roleNames)
//            {
//                var roleExist = await roleManager.RoleExistsAsync(roleName);

//                if (!roleExist)
//                {
//                    // Create the roles and seed them to the database
//                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

//                    if (roleResult.Succeeded)
//                    {
//                        logger.LogInformation($"Seeder: Role {roleName} created successfully.");
//                    }
//                    else
//                    {
//                        logger.LogError($"Seeder: Error creating role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
//                    }
//                }
//            }
//        }
//        private static async Task SeedAdminUser(UserManager<Admin> userManager, string username, string password, ILogger logger)
//        {
//            var adminUser = await userManager.FindByNameAsync(username);

//            if (adminUser == null)
//            {
//                if (password == null)
//                {
//                    logger.LogError("Admin user password cannot be null.");
//                    throw new ArgumentNullException(nameof(password), "Admin user password cannot be null.");
//                }

//                adminUser = new Admin
//                {
//                    UserName = username,
//                };

//                // Hash the password before creating the user
//                var hashedPassword = userManager.PasswordHasher.HashPassword(adminUser, password);
//                adminUser.PasswordHash = hashedPassword;

//                var result = await userManager.CreateAsync(adminUser);

//                if (result.Succeeded)
//                {
//                    logger.LogInformation("Admin user seeded successfully.");
//                }
//                else
//                {
//                    logger.LogError($"Error seeding admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//                }
//            }
//            else
//            {
//                logger.LogInformation("Admin user already exists.");
//            }
//        }





//        private static async Task SeedPatients(IServiceProvider serviceProvider, ILogger logger)
//        {
//            logger.LogInformation("Seeder: Start seeding patients.");

//            try
//            {
//                var dbContext = (ApplicationDbContext)serviceProvider.GetService(typeof(ApplicationDbContext));

//                if (dbContext != null)
//                {
//                    for (int i = 1; i <= 10; i++)
//                    {
//                        var patient = new Patient
//                        {
//                            FirstName = $"PatientFirstName{i}",
//                            Surname = $"PatientSurname{i}",
//                            HealthCardID = $"HealthCardID{i}",
//                            DateOfBirth = DateTime.Now.AddYears(-30).AddDays(i),
//                            Postcode = $"Postcode{i}"
//                        };

//                        dbContext.Patients.Add(patient);
//                        logger.LogInformation($"Seeder: Patient {i} added to context.");
//                    }

//                    await dbContext.SaveChangesAsync();
//                    logger.LogInformation("Seeder: Save changes to database.");
//                }
//                else
//                {
//                    logger.LogError("Seeder: Could not get DbContext for seeding patients.");
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.LogError($"Seeder: An error occurred while seeding patients. {ex}");
//            }

//            logger.LogInformation("Seeder: End seeding patients.");
//        }

//    }
//}
