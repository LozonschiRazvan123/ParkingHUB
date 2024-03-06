using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.Interface;
using ParkingHUB.Pagination;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Repository
{
    public class PaginationRepository<T> : IPagination<T>
    {
        private readonly DataContext _context;
        public PaginationRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<PageResult<T>> GetParkingListPagination(PaginationPage filter)
        {

            var query = _context.ParkingVehicles
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
                });

            var count = await query.CountAsync();

            var results = await query
                .OrderBy(p => p.Location)
                .Skip((filter.pageNumber - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .ToListAsync();

            var totalPages = (int)System.Math.Ceiling(count / (double)filter.pageSize);
            var previousPage = filter.pageNumber > 1 ? filter.pageNumber - 1 : (int?)null;
            var nextPage = filter.pageNumber < totalPages ? filter.pageNumber + 1 : (int?)null;

            return new PageResult<T>
            {
                Results = results.Cast<T>().ToList(),
                TotalCount = count,
                PageSize = filter.pageSize,
                CurrentPage = filter.pageNumber,
                TotalPages = totalPages,
                PreviousPage = previousPage,
                NextPage = nextPage
            };
        }
    }
}
