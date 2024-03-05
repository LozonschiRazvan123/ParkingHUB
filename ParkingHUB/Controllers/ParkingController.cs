using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Pagination;
using ParkingHUB.Repository;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Controllers
{
    public class ParkingController : Controller
    {
        private readonly IParking _parking;
        private readonly IPagination<ParkingListViewModel> _paginationRepository;
        private readonly DataContext _dataContext;
        public ParkingController(IParking parking, IPagination<ParkingListViewModel> paginationRepository, DataContext dataContext)
        {
            _parking = parking;
            _paginationRepository = paginationRepository;
            _dataContext = dataContext;
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

        public async Task<IActionResult> AddParking(ParkingListViewModel parking)
        {
            if (ModelState.IsValid)
            {
                
                if(parking.AvailableSlot > 0)
                {
                    parking.AvailableSlot--;
                    _dataContext.SaveChanges();
                    _parking.CreateParking(parking);
                }
                return RedirectToAction("ParkingListVehicle", new { location = parking.Location });
            }
            return View(parking);
        }

        private int GenerateParkingId()
        {
            return _dataContext.Parkings.Max(p => p.Id) + 1; 
        }

    }
}
