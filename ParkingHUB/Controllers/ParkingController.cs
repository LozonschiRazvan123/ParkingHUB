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

            var parking = await _dataContext.Parkings.FirstOrDefaultAsync(p => p.Location == location);
            var totalSlot = parking != null ? parking.TotalSlot : 0;

            ViewBag.TotalSlot = totalSlot;

            return View(pageResult);
        }

        public async Task<IActionResult> AddParking([FromBody] ParkingListViewModel parking, int pageNumber = 1, int pageSize = 10)
        {
            var filter = new PaginationPage
            {
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            if (ModelState.IsValid)
            {
                var parkingDetails = await _dataContext.Parkings.FirstOrDefaultAsync(p => p.Location == parking.Location);

                if (parkingDetails != null && parkingDetails.AvailableSlot > 0)
                {
                    if (parking.CheckIn < parking.CheckOut && (parking.CheckOut - parking.CheckIn).TotalHours % 1 == 0)
                    {
                        parkingDetails.AvailableSlot--;
                        parking.ParkingFee = CalculateParkingPrice(parking.CheckIn, parking.CheckOut, parkingDetails.Price);

                        _dataContext.Parkings.Update(parkingDetails);
                        await _dataContext.SaveChangesAsync();

                        if (_parking.CreateParking(parking))
                        {
                            return Json(new { showToast = true });
                        }
                        else
                        {
                            // În cazul în care nu putem crea parcare, ar trebui să ne asigurăm că slotul disponibil este restabilit
                            parkingDetails.AvailableSlot++;
                            _dataContext.Parkings.Update(parkingDetails);
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The difference between CheckIn and CheckOut should be an integer number of hours.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No available parking slots.");
                }
            }

            return Json(new { showToast = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }



        private int GenerateParkingId()
        {
            return _dataContext.Parkings.Max(p => p.Id) + 1; 
        }

        public double CalculateParkingPrice(DateTime checkIn, DateTime checkOut, double pricePerHour)
        {
            TimeSpan duration = checkOut - checkIn;
            double hours = duration.TotalHours;
            double totalPrice = pricePerHour * hours;
            return totalPrice;
        }


        public async Task<IActionResult> UpdateParking(int parkingId, IFormFile image, ParkingDTO model)
        {
            var parking = await _parking.GetParkingsId(parkingId);
            if (parking == null)
            {
                throw new Exception("Parking not found");
            }

            parking.Location = model.Location;
            parking.TotalSlot = model.TotalSlot;
            parking.AvailableSlot = model.AvailableSlot;
            parking.Price = model.Price;

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    parking.Image = memoryStream.ToArray();
                }
            }
            bool updateSuccessful = _parking.UpdateParkingImage(parking);
            if (updateSuccessful)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> EditParking(int parkingId)
        {
            var parking = await _parking.GetParkingsId(parkingId);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var parking = await _parking.GetParkingsId(id);
            if (parking == null || parking.Image == null)
            {
                return NotFound();
            }

            return File(parking.Image, "image/jpeg");
        }
    }
}
