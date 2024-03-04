using Microsoft.AspNetCore.Mvc;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Pagination;
using ParkingHUB.Repository;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Controllers
{
    public class ParkingController : Controller
    {
        private readonly IParking _parking;
        private readonly IPagination<ParkingListViewModel> _paginationRepository;
        public ParkingController(IParking parking, IPagination<ParkingListViewModel> paginationRepository)
        {
            _parking = parking;
            _paginationRepository = paginationRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ParkingDTO> parkings = await _parking.GetParkings();
            return View(parkings);
        }

        public async Task<IActionResult> ParkingList(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new PaginationPage
            {
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            var pageResult = await _paginationRepository.GetParkingListPagination(filter);

            return View(pageResult);
        }
        public async Task<IActionResult> ParkingListVehicle(string location, int pageNumber = 1, int pageSize = 10)
        {
            var filter = new PaginationPage
            {
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            var pageResult = await _parking.GetParkingVehicleInLocation(location, filter);
            ViewBag.Location = location;
            return View(pageResult);
        }

    }
}
