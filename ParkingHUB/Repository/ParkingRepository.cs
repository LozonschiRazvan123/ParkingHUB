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
            var parkingDetails = _context.Parkings.FirstOrDefault(p => p.Location == parking.Location);

            if (parkingDetails != null)
            {
                var vehicleEntity = new Vehicle
                {
                    PlateLicence = parking.PlateLicense,
                    CheckIn = parking.CheckIn,
                    CheckOut = parking.CheckOut,
                    ParkingFee = parking.ParkingFee
                };

                var parkingVehicleEntity = new ParkingVehicle
                {
                    Parking = parkingDetails,
                    Vehicle = vehicleEntity,
                    NumberParking = parking.NumberParking,
                    IsOcuppied = true
                };

                _context.ParkingVehicles.Add(parkingVehicleEntity);

                return Save();
            }

            return false;
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
                Image = p.Image
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
            var parkingDetails = await _context.Parkings.FirstOrDefaultAsync(p => p.Location == location);
            if (parkingDetails == null)
            {
                return new PageResult<ParkingListViewModel>
                {
                    Results = new List<ParkingListViewModel>(),
                    TotalCount = 0,
                    PageSize = filter.pageSize,
                    CurrentPage = filter.pageNumber,
                    TotalPages = 0,
                    PreviousPage = null,
                    NextPage = null
                };
            }

            var totalCount = parkingDetails.TotalSlot;
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.pageSize);

            var startIndex = (filter.pageNumber - 1) * filter.pageSize;
            var endIndex = Math.Min(startIndex + filter.pageSize, totalCount);

            var parkingSlots = await _context.ParkingVehicles
                                        .Where(pv => pv.Parking.Location == location)
                                        .Select(pv => pv.NumberParking) 
                                        .ToListAsync();

            var totalSlotList = Enumerable.Range(startIndex, endIndex - startIndex)
                .Select(i => new ParkingListViewModel
                {
                    TotalSlot = i + 1,
                    IsOccupied = parkingSlots.Contains(i + 1) 
                })
                .ToList();

            var previousPage = filter.pageNumber > 1 ? filter.pageNumber - 1 : (int?)null;
            var nextPage = filter.pageNumber < totalPages ? filter.pageNumber + 1 : (int?)null;

            return new PageResult<ParkingListViewModel>
            {
                Results = totalSlotList,
                TotalCount = totalCount,
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

        public bool DeleteParking(int parkingId)
        {
            var parking =  _context.Vehicles.FirstOrDefault(p => p.Id == parkingId);
            if (parking != null)
            {
                _context.Vehicles.Remove(parking);
                return Save();
            }
            return false;
        }

        public async Task<ParkingDTO> GetParkingsId(int id)
        {
            var parking = _context.Parkings.FirstOrDefault(p => p.Id == id);

            if (parking == null)
            {
                throw new Exception($"Parking with id {id} not found.");
            }

            return new ParkingDTO
            {
                Id = parking.Id,
                TotalSlot = parking.TotalSlot,
                AvailableSlot = parking.AvailableSlot,
                Price = parking.Price,
                Location = parking.Location,
                Image = parking.Image 
            };
        }

        public bool UpdateParkingImage(ParkingDTO parking)
        {
            var existParking = _context.Parkings.FirstOrDefault(u => u.Id == parking.Id);

            if (existParking != null)
            {

                existParking.AvailableSlot = parking.AvailableSlot;
                existParking.Image = parking.Image;
                existParking.Location = parking.Location;
                existParking.Image = parking.Image;
                existParking.Price = parking.Price;
                existParking.TotalSlot = parking.TotalSlot;
                existParking.Id = parking.Id;
                _context.Update(existParking);
                return Save();
            }

            return false;
        }

        public Task<byte[]> GetImageById(int id)
        {
            var parking = _context.Parkings.FirstOrDefault(img => img.Id == id);
            if (parking != null)
            {
                return Task.FromResult(parking.Image); 
            }
            else
            {
                throw new Exception("Parking not found");
            }
        }
    }
}
