using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;

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
                Price= p.Price,
                CheckIn = p.CheckIn,
                CheckOut= p.CheckOut,

            });
        }
    }
}
