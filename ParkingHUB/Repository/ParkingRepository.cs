using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Repository
{
    public class ParkingRepository : IParking
    {
        private readonly DataContext _context;
        public ParkingRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ParkingDTO>> GetParkings()
        {
            return  _context.Parkings.Select(p => new ParkingDTO
            {
                Id = p.Id,
                AvailableSlot = p.AvailableSlot,
                TotalSlot = p.TotalSlot,
                Location = p.Location,
                Price= p.Price
            });
        }

        public async Task<IEnumerable<ParkingListViewModel>> GetParkingVehicle()
        {
            return  _context.ParkingVehicles
                        .Include(vp => vp.Vehicle)
                        .Include(vp => vp.Parking)
                        .Select(vp => new ParkingListViewModel
                        {
                            ParkingId = vp.Parking.Id,
                            Location = vp.Parking.Location,
                            PlateLicense = vp.Vehicle.PlateLicence,
                            CheckIn = vp.Vehicle.CheckIn,
                            CheckOut = vp.Vehicle.CheckOut
    
                        });

        }
    }
}
