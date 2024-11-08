using Microsoft.AspNetCore.Identity;
using TomNam.Models;

namespace TomNam
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
    
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
    
                    // Create roles if they do not exist
                    string[] roleNames = { "Customer", "Owner", "Admin" };
                    foreach (var roleName in roleNames)
                    {
                        var roleExist = await roleManager.RoleExistsAsync(roleName);
                        if (!roleExist)
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }
    
                    // Create admin if it does not exist
                    if (await userManager.FindByEmailAsync("admin@admin.com") == null)
                    {
                        var admin = new User
                        {
                            Email = "admin@admin.com",
                            UserName = "admin",
                            FirstName = "admin",
                            LastName = "admin",
                            SecurityStamp = Guid.NewGuid().ToString()
                        };
    
                        await userManager.CreateAsync(admin, "Admin@123");
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating roles and admin user: {ex.Message}");
                }
            }
    
            await host.RunAsync();
        }
    
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}