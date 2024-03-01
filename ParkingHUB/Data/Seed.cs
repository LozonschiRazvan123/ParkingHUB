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
                
                if(!context.Parkings.Any())
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
                            CheckOut = DateTime.Parse("2024-03-01 18:00"),
                        },

                        new Parking
                        {
                            Price = 7.00,
                            TotalSlot = 20,
                            AvailableSlot = 15,
                            Location = "Piata Centrala, Strada Libertatii nr. 45",
                            CheckIn = DateTime.Parse("2024-02-29 09:30"),
                            CheckOut = DateTime.Parse("2024-02-29 18:30")
                        },

                        new Parking
                        {
                            Price = 15.75,
                            TotalSlot = 12,
                            AvailableSlot = 11,
                            Location = "Aeroportul International, Terminalul 2",
                            CheckIn = DateTime.Parse("2024-02-28 17:00"),
                            CheckOut = DateTime.Parse("2024-03-02 12:00")
                        },

                        new Parking
                        {
                            Price = 3.50,
                            TotalSlot = 111,
                            AvailableSlot = 108,
                            Location = "Cladirea Office Park, Bulevardul Victoriei nr. 67",
                            CheckIn = DateTime.Parse("2024-03-01 10:15"),
                            CheckOut = DateTime.Parse("2024-03-01 11:45")
                        },

                        new Parking
                        {
                            Price = 80.00,
                            TotalSlot = 78,
                            AvailableSlot = 76,
                            Location = "Strada Linistii nr. 10",
                            CheckIn = DateTime.Parse("2024-02-01 00:00"),
                            CheckOut = DateTime.Parse("2024-02-29 23:59")
                        }

                        }) ;
                    context.SaveChanges();
                }
            }
        }
    }
}
