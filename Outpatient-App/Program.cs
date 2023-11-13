//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.EntityFrameworkCore;
//using Outpatient_App.Data;
//using Outpatient_App.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using Serilog;
//using Serilog.Events;
//using System;
//using System.Globalization;
//using Microsoft.AspNetCore.Mvc.Razor;
//using Microsoft.AspNetCore.Localization;
//using Microsoft.VisualBasic;
//using System.Collections.ObjectModel;
//using System.Threading.Tasks;
//using Outpatient_App.Hubs;

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        var builder = WebApplication.CreateBuilder(args);

//        // Add services to the container.
//        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//        builder.Services.AddDbContext<ApplicationDbContext>(options =>
//            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//            .AddRoles<IdentityRole>()
//            .AddEntityFrameworkStores<ApplicationDbContext>();

//        builder.Services.AddControllersWithViews()
//            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
//            .AddDataAnnotationsLocalization();
//        builder.Services.AddHttpContextAccessor();

//        builder.Services.AddSignalR();

//        // Configure supported cultures
//        var supportedCultures = new[]
//        {
//            new CultureInfo("en"), // English
//            new CultureInfo("fr"), // French
//            new CultureInfo("es"), // Spanish
//        };

//        builder.Services.Configure<RequestLocalizationOptions>(options =>
//        {
//            options.DefaultRequestCulture = new RequestCulture("en");
//            options.SupportedCultures = supportedCultures;
//            options.SupportedUICultures = supportedCultures;
//        });

//        // Configure Serilog for file logging
//        builder.Services.AddLogging(logging =>
//        {
//            logging.AddSerilog(new LoggerConfiguration()
//                .WriteTo.Console()
//                .WriteTo.File("Logs/myapp.log", rollingInterval: RollingInterval.Day)
//                .CreateLogger());
//        });

//        var app = builder.Build();

//        // Configure the HTTP request pipeline.
//        if (!app.Environment.IsDevelopment())
//        {
//            app.UseExceptionHandler("/Home/Error");
//            app.UseHsts();
//        }

//        app.UseRequestLocalization();

//        app.UseHttpsRedirection();
//        app.UseStaticFiles();

//        app.UseRouting();

//        app.UseAuthentication(); // Add this line for authentication
//        app.UseAuthorization();


//        // Use SignalR
//        app.MapHub<UpdateHub>("/updateHub"); // Replace YourHubClass and yourHubPath with your actual Hub class and path

//        app.MapRazorPages();

//        app.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");

//        app.MapControllerRoute(
//            name: "disclaimer",
//            pattern: "/Home/Disclaimer/{lang}",
//            defaults: new { controller = "Home", action = "Disclaimer" }
//        );

//        // Seed the roles
//        using (var scope = app.Services.CreateScope())
//        {
//            var services = scope.ServiceProvider;
//            try
//            {
//                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//                var roles = new[] { "Admin", "Staff" };

//                foreach (var role in roles)
//                {
//                    if (!await roleManager.RoleExistsAsync(role))
//                        await roleManager.CreateAsync(new IdentityRole(role));
//                }

//                var logger = services.GetRequiredService<ILogger<Program>>();
//            }
//            catch (Exception ex)
//            {
//                var logger = services.GetRequiredService<ILogger<Program>>();
//                logger.LogError(ex, "An error occurred while seeding the database.");
//            }
//        }

//        // creating admin login detail
//        using (var scope = app.Services.CreateScope())
//        {
//            var services = scope.ServiceProvider;
//            try
//            {
//                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
//                string email = "admin@admin.com";
//                string password = "Password1#";

//                if (await userManager.FindByEmailAsync(email) == null)
//                {
//                    var user = new IdentityUser
//                    {
//                        UserName = email,
//                        Email = email
//                    };

//                    await userManager.CreateAsync(user, password);
//                    await userManager.AddToRoleAsync(user, "Admin");
//                }

//                var logger = services.GetRequiredService<ILogger<Program>>();
//            }
//            catch (Exception ex)
//            {
//                var logger = services.GetRequiredService<ILogger<Program>>();
//                logger.LogError(ex, "An error occurred while seeding the database.");
//            }
//        }

//        app.Run();
//    }
//}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Outpatient_App.Data;
using Outpatient_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Outpatient_App.updateHubs;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSignalR();

        // Configure supported cultures
        var supportedCultures = new[]
        {
            new CultureInfo("en"), // English
            new CultureInfo("fr"), // French
            new CultureInfo("es"), // Spanish
        };

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        // Configure Serilog for file logging
        builder.Services.AddLogging(logging =>
        {
            logging.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/myapp.log", rollingInterval: RollingInterval.Day)
                .CreateLogger());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseRequestLocalization();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication(); // Add this line for authentication
        app.UseAuthorization();

        // Use SignalR
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<UpdateHub>("/updateHub"); // Replace YourHubClass and yourHubPath with your actual Hub class and path

            endpoints.MapRazorPages();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "disclaimer",
                pattern: "/Home/Disclaimer/{lang}",
                defaults: new { controller = "Home", action = "Disclaimer" }
            );
        });

        // Seed the roles
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Staff" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var logger = services.GetRequiredService<ILogger<Program>>();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        // creating admin login detail
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                string email = "admin@admin.com";
                string password = "Password1#";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = email,
                        Email = email
                    };

                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");
                }

                var logger = services.GetRequiredService<ILogger<Program>>();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        await app.RunAsync();

    }
}
