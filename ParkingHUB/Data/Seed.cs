using Microsoft.AspNetCore.Identity;
using ParkingHUB.Models;

namespace ParkingHUB.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {


            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                var context = serviceScope.ServiceProvider.GetService<DataContext>();

                context.Database.EnsureCreated();

                if (!context.Parkings.Any())
                {
                    context.Parkings.AddRange(new List<Parking>()
                    {
                        new Parking
                        {
                            Price = 10.50,
                            TotalSlot = 101,
                            AvailableSlot = 97,
                            Location = "Mall X, Strada Principala nr. 123",
                            CheckIn = DateTime.Parse("2024-03-01 08:00"),
                            CheckOut = DateTime.Parse("2024-03-01 10:00"),
                        },

                        new Parking
                        {
                            Price = 7.00,
                            TotalSlot = 20,
                            AvailableSlot = 15,
                            Location = "Piata Centrala, Strada Libertatii nr. 45",
                            CheckIn = DateTime.Parse("2024-02-29 09:30"),
                            CheckOut = DateTime.Parse("2024-02-29 11:30")
                        },

                        new Parking
                        {
                            Price = 15.75,
                            TotalSlot = 12,
                            AvailableSlot = 11,
                            Location = "Aeroportul International, Terminalul 2",
                            CheckIn = DateTime.Parse("2024-02-28 17:00"),
                            CheckOut = DateTime.Parse("2024-03-02 18:00")
                        },

                        new Parking
                        {
                            Price = 3.50,
                            TotalSlot = 111,
                            AvailableSlot = 108,
                            Location = "Cladirea Office Park, Bulevardul Victoriei nr. 67",
                            CheckIn = DateTime.Parse("2024-03-01 10:15"),
                            CheckOut = DateTime.Parse("2024-03-01 12:15")
                        },

                        new Parking
                        {
                            Price = 80.00,
                            TotalSlot = 78,
                            AvailableSlot = 76,
                            Location = "Strada Linistii nr. 10",
                            CheckIn = DateTime.Parse("2024-02-01 08:00"),
                            CheckOut = DateTime.Parse("2024-02-1 11:00")
                        },

                        new Parking
                        {
                            Price = 50.00,
                            TotalSlot = 88,
                            AvailableSlot = 86,
                            Location = "Strada Petru Rares nr. 110",
                            CheckIn = DateTime.Parse("2024-02-01 08:10"),
                            CheckOut = DateTime.Parse("2024-02-1 12:10")
                        }

                        });
                    context.SaveChanges();
                }

                if (!context.Vehicles.Any())
                {
                    context.Vehicles.AddRange(new List<Vehicle>()
                    {
                        new Vehicle
                        {
                            PlateLicence = "IS 21 MAY",
                        },

                        new Vehicle
                        {
                            PlateLicence = "IS 20 OCT"
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 19 RAZ"
                        },

                        new Vehicle
                        {
                            PlateLicence = "BC 01 IIS"
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 99 TAR"
                        },

                        new Vehicle
                        {
                            PlateLicence = "VS 12 PUI"
                        },

                        new Vehicle
                        {
                            PlateLicence = "GL 44 FDL"
                        },
                        
                        new Vehicle
                        {
                            PlateLicence = "BT 32 XYZ"
                        },

                        new Vehicle
                        {
                            PlateLicence = "VS 22 IKJ"
                        },

                        new Vehicle
                        {
                            PlateLicence = "GL 32 QWL"
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 78 DDD"
                        }
                    });
                    context.SaveChanges();
                }
            }

        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminUserEmail = "razvanlozonschi123@gmail.com";

                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "RazvanLozonschi",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");

                    var admin = await userManager.FindByEmailAsync("razvanlozonschi123@gmail.com");

                    User Dbuser = context.Users.Where(s => s.Email == newAdminUser.Email).FirstOrDefault();
                    await userManager.AddToRoleAsync(Dbuser, UserRoles.Admin);
                }

                string appUserEmail = "experimentfacultate@gmail.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new User()
                    {
                        UserName = "UserName",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                    };

                    await userManager.CreateAsync(newAppUser, "Coding@1234?");

                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }
    }
}
