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
                        },

                        new Parking
                        {
                            Price = 7.00,
                            TotalSlot = 20,
                            AvailableSlot = 15,
                            Location = "Piata Centrala, Strada Libertatii nr. 45",
                        },

                        new Parking
                        {
                            Price = 15.75,
                            TotalSlot = 12,
                            AvailableSlot = 11,
                            Location = "Aeroportul International, Terminalul 2",
                        },

                        new Parking
                        {
                            Price = 3.50,
                            TotalSlot = 112,
                            AvailableSlot = 108,
                            Location = "Cladirea Office Park, Bulevardul Victoriei nr. 67",
                        },

                        new Parking
                        {
                            Price = 80.00,
                            TotalSlot = 78,
                            AvailableSlot = 76,
                            Location = "Strada Linistii nr. 10",
                        },

                        new Parking
                        {
                            Price = 50.00,
                            TotalSlot = 88,
                            AvailableSlot = 86,
                            Location = "Strada Petru Rares nr. 110",
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
                            CheckIn = DateTime.Parse("2024-03-01 08:00"),
                            CheckOut = DateTime.Parse("2024-03-01 10:00"),
                            ParkingFee = 21,
                        },

                        new Vehicle
                        {
                            PlateLicence = "IS 20 OCT",
                            CheckIn = DateTime.Parse("2024-03-01 09:30"),
                            CheckOut = DateTime.Parse("2024-03-01 11:30"),
                            ParkingFee = 21,
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 19 RAZ",
                            CheckIn = DateTime.Parse("2024-03-02 17:00"),
                            CheckOut = DateTime.Parse("2024-03-02 18:00"),
                            ParkingFee = 10.5,
                        },

                        new Vehicle
                        {
                            PlateLicence = "BC 01 IIS",
                            CheckIn = DateTime.Parse("2024-03-01 10:15"),
                            CheckOut = DateTime.Parse("2024-03-01 12:15"),
                            ParkingFee = 21, 
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 99 TAR",
                            CheckIn = DateTime.Parse("2024-03-01 10:15"),
                            CheckOut = DateTime.Parse("2024-03-01 12:15"),
                            ParkingFee = 14,
                        },

                        new Vehicle
                        {
                            PlateLicence = "VS 12 PUI",
                            CheckIn = DateTime.Parse("2024-03-01 08:00"),
                            CheckOut = DateTime.Parse("2024-03-1 11:00"),
                            ParkingFee = 14,
                        },

                        new Vehicle
                        {
                            PlateLicence = "GL 44 FDL",
                            CheckIn = DateTime.Parse("2024-03-01 08:00"),
                            CheckOut = DateTime.Parse("2024-03-1 11:00"),
                            ParkingFee = 21,
                        },

                        new Vehicle
                        {
                            PlateLicence = "BT 32 XYZ",
                            CheckIn = DateTime.Parse("2024-03-01 08:10"),
                            CheckOut = DateTime.Parse("2024-03-1 12:10"),
                            ParkingFee = 14,
                        },

                        new Vehicle
                        {
                            PlateLicence = "VS 22 IKJ",
                            CheckIn = DateTime.Parse("2024-03-02 17:10"),
                            CheckOut = DateTime.Parse("2024-03-02 20:10"),
                            ParkingFee = 21,
                        },

                        new Vehicle
                        {
                            PlateLicence = "GL 32 QWL",
                            CheckIn = DateTime.Parse("2024-03-01 08:10"),
                            CheckOut = DateTime.Parse("2024-03-01 12:10"),
                            ParkingFee = 31.5,
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 78 DDD",
                            CheckIn = DateTime.Parse("2024-03-02 10:10"),
                            CheckOut = DateTime.Parse("2024-03-02 12:10"),
                            ParkingFee = 7,
                        },

                        new Vehicle
                        {
                            PlateLicence = "BC 22 PQR",
                            CheckIn = DateTime.Parse("2024-03-02 9:23"),
                            CheckOut = DateTime.Parse("2024-03-02 12:23"),
                            ParkingFee = 10.5,
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 67 LIV",
                            CheckIn = DateTime.Parse("2024-03-02 10:30"),
                            CheckOut = DateTime.Parse("2024-03-02 12:30"),
                            ParkingFee = 7,
                        },

                        new Vehicle
                        {
                            PlateLicence = "VS 17 FCD",
                            CheckIn = DateTime.Parse("2024-03-02 17:00"),
                            CheckOut = DateTime.Parse("2024-03-02 20:00"),
                            ParkingFee = 10.5,
                        },

                        new Vehicle
                        {
                            PlateLicence = "NT 98 PQR",
                            CheckIn = DateTime.Parse("2024-03-02 11:00"),
                            CheckOut = DateTime.Parse("2024-03-02 12:00"),
                            ParkingFee = 80,
                        },

                        new Vehicle
                        {
                            PlateLicence = "IS 27 FCS",
                            CheckIn = DateTime.Parse("2024-03-01 9:23"),
                            CheckOut = DateTime.Parse("2024-03-01 12:23"),
                            ParkingFee = 240,
                        },

                        new Vehicle
                        {
                            PlateLicence = "IS 29 CFR",
                            CheckIn = DateTime.Parse("2024-03-01 19:00"),
                            CheckOut = DateTime.Parse("2024-03-01 21:00"),
                            ParkingFee = 100,
                        },

                        new Vehicle
                        {
                            PlateLicence = "IS 31 PNI",
                            CheckIn = DateTime.Parse("2024-03-01 10:00"),
                            CheckOut = DateTime.Parse("2024-03-01 21:00"),
                            ParkingFee = 550,
                        }

                    });
                    context.SaveChanges();
                }

                if (!context.ParkingVehicles.Any())
                {
                    context.ParkingVehicles.AddRange(new List<ParkingVehicle>()
                    {
                        new ParkingVehicle
                        {
                            ParkingId = 1,
                            VehicleId = 1,
                            NumberParking = 1,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 1,
                            VehicleId = 2,
                            NumberParking = 2,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 1,
                            VehicleId = 3,
                            NumberParking = 3,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 1,
                            VehicleId = 4,
                            NumberParking = 5,
                            IsOcuppied = true,
                        },
                        

                        new ParkingVehicle
                        {
                            ParkingId = 2,
                            VehicleId = 5,
                            NumberParking = 1,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 2,
                            VehicleId = 6,
                            NumberParking = 2,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 2,
                            VehicleId = 7,
                            NumberParking = 7,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle 
                        { 
                            ParkingId = 2,
                            VehicleId = 8,
                            NumberParking = 5,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 2,
                            VehicleId = 9,
                            NumberParking = 9,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 3,
                            VehicleId = 10,
                            NumberParking = 1,
                            IsOcuppied = true
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 4,
                            VehicleId = 11,
                            NumberParking = 2,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 4,
                            VehicleId = 12,
                            NumberParking = 3,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 4,
                            VehicleId = 13,
                            NumberParking = 13,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 4,
                            VehicleId = 14,
                            NumberParking = 11,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 5,
                            VehicleId = 15,
                            NumberParking = 1,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 5,
                            VehicleId = 16,
                            NumberParking= 2,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 6,
                            VehicleId = 17,
                            NumberParking= 3,
                            IsOcuppied = true,
                        },

                        new ParkingVehicle
                        {
                            ParkingId = 6,
                            VehicleId = 18,
                            NumberParking= 18,
                            IsOcuppied = true,
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
