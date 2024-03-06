using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Pagination;
using ParkingHUB.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ParkingHUB.Repository
{
    public class ParkingRepository : IParking
    {
        private readonly DataContext _context;
        private readonly PaginationRepository<ParkingListViewModel> _pagination;
        public ParkingRepository(DataContext context, PaginationRepository<ParkingListViewModel> pagination)
        {
            _context = context;
            _pagination = pagination;
        }

        public bool Save()
        {
            var changes = _context.SaveChanges();
            return changes > 0 ? true : false;
        }

        public bool CreateParking(ParkingListViewModel parking)
        {
            var parkingDetails =  _context.Parkings
                    .FirstOrDefaultAsync(p => p.Location == parking.Location);

            var vehicleEntity = new Vehicle
            {
                Id = parking.ParkingId,
                PlateLicence = parking.PlateLicense,
                CheckIn = parking.CheckIn,
                CheckOut = parking.CheckOut,
                ParkingFee = parking.ParkingFee
            };

            var parkingVehicleEntity = new ParkingVehicle
            {
                Parking = parkingDetails.Result,
                Vehicle = vehicleEntity
            };

            _context.ParkingVehicles.Add(parkingVehicleEntity);
            return Save();
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

            });
        }

        public async Task<IEnumerable<ParkingListViewModel>> GetParkingVehicle()
        {
            return  await _context.ParkingVehicles
                        .Include(vp => vp.Vehicle)
                        .Include(vp => vp.Parking)
                        .Select(vp => new ParkingListViewModel
                        {
                            ParkingId = vp.Parking.Id,
                            Location = vp.Parking.Location,
                            PlateLicense = vp.Vehicle.PlateLicence,
                            CheckIn = vp.Vehicle.CheckIn,
                            CheckOut = vp.Vehicle.CheckOut,
                            ParkingFee = vp.Vehicle.ParkingFee
    
                        }).ToListAsync();

        }


        public async Task<PageResult<ParkingListViewModel>> GetParkingVehicleInLocation(string location, PaginationPage filter)
        {
            var parkings =  _context.ParkingVehicles
                        .Include(vp => vp.Vehicle)
                        .Include(vp => vp.Parking)
                        .Where(vp => vp.Parking.Location == location)
                        .Select(vp => new ParkingListViewModel
                        {
                            ParkingId = vp.Parking.Id,
                            Location = vp.Parking.Location,
                            PlateLicense = vp.Vehicle.PlateLicence,
                            CheckIn = vp.Vehicle.CheckIn,
                            CheckOut = vp.Vehicle.CheckOut,
                            ParkingFee = vp.Vehicle.ParkingFee

                        });

            var count = await parkings.CountAsync();

            var results = await parkings
                .OrderBy(p => p.Location)
                .Skip((filter.pageNumber - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .ToListAsync();

            var totalPages = (int)System.Math.Ceiling(count / (double)filter.pageSize);
            var previousPage = filter.pageNumber > 1 ? filter.pageNumber - 1 : (int?)null;
            var nextPage = filter.pageNumber < totalPages ? filter.pageNumber + 1 : (int?)null;

            return new PageResult<ParkingListViewModel>
            {
                Results = results.Cast<ParkingListViewModel>().ToList(),
                TotalCount = count,
                PageSize = filter.pageSize,
                CurrentPage = filter.pageNumber,
                TotalPages = totalPages,
                PreviousPage = previousPage,
                NextPage = nextPage
            };
        }

        public async Task<IEnumerable<ParkingListViewModel>> GetParkingId(int id)
        {
            var result = await _context.ParkingVehicles
                       .Include(vp => vp.Vehicle)
                       .Include(vp => vp.Parking)
                       .Where(vp => vp.ParkingId == id)
                       .Select(vp => new ParkingListViewModel
                       {
                           ParkingId = vp.Parking.Id,
                           Location = vp.Parking.Location,
                           PlateLicense = vp.Vehicle.PlateLicence,
                           CheckIn = vp.Vehicle.CheckIn,
                           CheckOut = vp.Vehicle.CheckOut,
                           ParkingFee = vp.Vehicle.ParkingFee

                       }).ToListAsync();
            return result;
        }
    }
}
