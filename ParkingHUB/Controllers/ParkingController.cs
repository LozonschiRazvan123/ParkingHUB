using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Pagination;
using ParkingHUB.Repository;
using ParkingHUB.ViewModel;
using System.Security.Claims;

namespace ParkingHUB.Controllers
{
    public class ParkingController : Controller
    {
        private readonly IParking _parking;
        private readonly IPagination<ParkingListViewModel> _paginationRepository;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public ParkingController(IParking parking, IPagination<ParkingListViewModel> paginationRepository, DataContext dataContext, IHttpContextAccessor httpContext, DataContext context, UserManager<User> userManager)
        {
            _parking = parking;
            _paginationRepository = paginationRepository;
            _dataContext = dataContext;
            _httpContext = httpContext;
            _context = context;
            _userManager = userManager;
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

            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ModelState.IsValid)
            {
                var parkingDetails = await _dataContext.Parkings.FirstOrDefaultAsync(p => p.Location == parking.Location);

                if (parkingDetails != null && parkingDetails.AvailableSlot > 0)
                {
                    parkingDetails.AvailableSlot--;

                    _dataContext.Parkings.Update(parkingDetails);
                    await _dataContext.SaveChangesAsync();

                    if (_parking.CreateParking(parking, userId))
                    {
                        return Json(new { showToast = true });
                    }
                    else
                    {
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

            ViewBag.Location = model.Location;
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

            ViewBag.Location = parking.Location;

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

        public async Task<IActionResult> CreateLocationForParking(ParkingDTO parking, IFormFile image)
        {

            if (!ModelState.IsValid)
            {
                return View(parking);
            }

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    parking.Image = memoryStream.ToArray();
                }
            }

            if (_parking.CreateLocationForParking(parking))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to create parking location.");
                return View(parking);
            }
        }

        public async Task<IActionResult> DeleteParking(int id)
        {
            var success = _parking.DeleteParking(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ExtendParkingVehicleTime()
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string requestBody = await reader.ReadToEndAsync();
                var requestData = JsonConvert.DeserializeObject<JObject>(requestBody);

                int vehicleId = requestData.Value<int>("vehicleId");
                DateTime newEndTime = requestData.Value<DateTime>("newEndTime");

                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);
                if (vehicle != null)
                {
                    bool parkingExtended = await _parking.ExtendParkingTime(vehicleId, newEndTime);
                    if (parkingExtended)
                    {
                        return Json(new { showToast = true });
                    }
                    else
                    {
                        return Json(new { showToast = false });
                    }
                }
                else
                {
                    return Json(new { showToast = false });
                }
            }
        }

        public async Task<IActionResult> Search(DateTime checkIn, DateTime checkOut, int pageNumber = 1, int pageSize = 10)
        {
            var pageResult = await _parking.Search(checkIn, checkOut, pageNumber, pageSize);
            return View("ParkingList", pageResult);
        }


        [HttpPost]
        public async Task<IActionResult> DownloadPdf(DateTime checkIn, DateTime checkOut)
        {
            var pdfBytes = await _parking.GenereateVehicleParkingPDF(checkIn, checkOut);

            return File(pdfBytes, "application/pdf", "vehicle_parking_report.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout([FromBody] ParkingListViewModel checkoutData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);

            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == checkoutData.VehicleId && v.UserId == userId);
            if (vehicle == null)
            {
                return Json(new { showToast = false });
            }

            var checkIn = vehicle.CheckIn;

            checkoutData.CheckIn = checkIn;

            var vehicleParking = _context.ParkingVehicles
                .Include(vp => vp.Parking)
                .Include(vp => vp.Vehicle)
                .FirstOrDefault(vp => vp.VehicleId == vehicle.Id);

            checkoutData.Price = vehicleParking.Parking.Price;
            vehicleParking.IsOcuppied = false;

            var parking = _context.Parkings.FirstOrDefault(p => p.Id == vehicleParking.ParkingId);
            if (parking != null)
            {
                parking.AvailableSlot++; 
            }

            

            var result = await _parking.ProcessCheckout(checkoutData);
            
            //await _context.SaveChangesAsync();

            if (result)
            {
                return Json(new { showToast = true });
            }
            else
            {
                return Json(new { showToast = false });
            }
            
            
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingChartsData()
        {
            var parkingData = await _parking.GetParkingDataForChart();
            if (parkingData != null && parkingData.Any())
            {
                return Json(parkingData);
            }
            return NotFound("No parking data available.");
        }

        [HttpGet]
        public IActionResult Charts()
        {
            return View();
        }


    }
}
