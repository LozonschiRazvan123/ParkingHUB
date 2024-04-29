using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Pagination;
using ParkingHUB.ViewModel;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Snippets.Font;
using System.Drawing;
using System.Security.Claims;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ParkingHUB.Repository
{
    public class ParkingRepository : IParking
    {
        private readonly DataContext _context;
        private readonly PaginationRepository<ParkingListViewModel> _pagination;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ParkingRepository(DataContext context, PaginationRepository<ParkingListViewModel> pagination, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _pagination = pagination;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Save()
        {
            var changes = _context.SaveChanges();
            return changes > 0 ? true : false;
        }

        public bool CreateParking(ParkingListViewModel parking, string userId)
        {
            var parkingDetails = _context.Parkings.FirstOrDefault(p => p.Location == parking.Location);

            if (parkingDetails != null)
            {
                var vehicleEntity = new Vehicle
                {
                    PlateLicence = parking.PlateLicense,
                    CheckIn = parking.CheckIn,
                    CheckOut = parking.CheckOut,
                    ParkingFee = parking.ParkingFee,
                    UserId = userId
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
            return _context.Parkings.Select(p => new ParkingDTO
            {
                Id = p.Id,
                AvailableSlot = p.AvailableSlot,
                TotalSlot = p.TotalSlot,
                Location = p.Location,
                Price = p.Price,
                Image = p.Image
            });
        }

        public async Task<IEnumerable<ParkingListViewModel>> GetParkingVehicle()
        {
            return await _context.ParkingVehicles
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
                                .Include(pv => pv.Parking)
                                .Include(pv => pv.Vehicle)
                                .Where(pv => pv.Parking.Location == location && pv.IsOcuppied)
                                .Select(pv => new { pv.NumberParking, pv.Vehicle.PlateLicence, pv.Vehicle.CheckIn, pv.Vehicle.CheckOut, pv.Vehicle.Id })
                                .ToListAsync();

            var totalSlotList = Enumerable.Range(startIndex, endIndex - startIndex)
                .Select(i =>
                {
                    var parkingSlot = parkingSlots.FirstOrDefault(p => p.NumberParking == i + 1);
                    double price = 0;
                    if (parkingSlot != null)
                    {
                        price = CalculateParkingPrice(parkingSlot.CheckIn, parkingSlot.CheckOut, parkingDetails.Price);
                    }
                    return new ParkingListViewModel
                    {
                        TotalSlot = i + 1,
                        IsOccupied = parkingSlot != null,
                        PlateLicense = parkingSlot != null ? parkingSlot.PlateLicence : null,
                        Price = price,
                        VehicleId = parkingSlot != null ? parkingSlot.Id : 0
                    };
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

        public double CalculateParkingPrice(DateTime checkIn, DateTime checkOut, double pricePerHour)
        {
            TimeSpan duration = checkOut - checkIn;
            double hours = duration.TotalHours;
            double totalPrice = pricePerHour * hours;
            return totalPrice;
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
            var parking = _context.Parkings.FirstOrDefault(p => p.Id == parkingId);
            if (parking != null)
            {
                _context.Parkings.Remove(parking);
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

        public bool CreateLocationForParking(ParkingDTO parkingDTO)
        {
            var newParking = new Parking
            {
                Location = parkingDTO.Location,
                TotalSlot = parkingDTO.TotalSlot,
                AvailableSlot = parkingDTO.AvailableSlot,
                Price = parkingDTO.Price,
                Image = parkingDTO.Image,
            };

            _context.Add(newParking);
            return Save();
        }
        public async Task<bool> ExtendParkingTime(int vehicleId, DateTime newEndTime)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);
            if (vehicle != null)
            {
                vehicle.CheckOut = newEndTime;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PageResult<ParkingListViewModel>> Search(DateTime checkIn, DateTime checkOut, int pageNumber = 1, int pageSize = 10)
        {
            var totalResults = await _context.ParkingVehicles
                               .Where(p => p.Vehicle.CheckIn >= checkIn && p.Vehicle.CheckOut <= checkOut)
                               .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            var parkings = await _context.ParkingVehicles
                .Include(p => p.Vehicle)
                .Include(p => p.Parking)
                .Where(p => p.Vehicle.CheckIn >= checkIn && p.Vehicle.CheckOut <= checkOut)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var pageResult = new PageResult<ParkingListViewModel>
            {
                Results = parkings.Select(p => new ParkingListViewModel
                {
                    Location = p.Parking.Location,
                    PlateLicense = p.Vehicle.PlateLicence,
                    CheckIn = p.Vehicle.CheckIn,
                    CheckOut = p.Vehicle.CheckOut,
                    ParkingFee = p.Vehicle.ParkingFee
                }).ToList(),
                TotalCount = totalResults,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = totalPages,

            };
            pageResult.CheckIn = checkIn;
            pageResult.CheckOut = checkOut;

            return pageResult;
        }

        public async Task<byte[]> GenereateVehicleParkingPDF(DateTime checkIn, DateTime checkOut)
        {
            var results = await _context.ParkingVehicles
                .Include(vp => vp.Vehicle)
                .Include(vp => vp.Parking)
                .Where(p => p.Vehicle.CheckIn >= checkIn && p.Vehicle.CheckOut <= checkOut)
                .ToListAsync();

            var document = new PdfDocument();

            PdfPage page = document.AddPage();
            page.Width = XUnit.FromCentimeter(29.7);
            page.Height = XUnit.FromCentimeter(42);

            XGraphics gfx = XGraphics.FromPdfPage(page);

            if (Capabilities.Build.IsCoreBuild)
            {
                GlobalFontSettings.FontResolver = new FailsafeFontResolver();
            }
            var font = new XFont("Arial", 10, XFontStyleEx.Bold);

            double[] columnWidths = { 50, 150, 150, 150, 150, 150 };
            double marginLeft = 10;
            double rowHeight = 20;

            XRect titleRect = new XRect(marginLeft, 10, page.Width - 20, rowHeight);
            gfx.DrawString("Vehicle Parking Report", font, XBrushes.Black, titleRect, XStringFormats.Center);

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            string currentUserName = currentUser != null ? currentUser.UserName : "Anonymous";
            string generatedBy = "Generated by: " + currentUserName;

            XRect generatedOnRect = new XRect(marginLeft, 30, page.Width - 20, rowHeight);
            XRect generatedByRect = new XRect(marginLeft, 50, page.Width - 20, rowHeight);

            gfx.DrawString($"Generated on: {currentDate}", font, XBrushes.Black, generatedOnRect, XStringFormats.TopLeft);
            gfx.DrawString(generatedBy, font, XBrushes.Black, generatedByRect, XStringFormats.TopLeft);

            double tableTopMargin = 80; 
            double currentY = tableTopMargin;

            XRect rect = new XRect(marginLeft, currentY, page.Width - 20, rowHeight);
            gfx.DrawString("Number", font, XBrushes.Black, new XRect(rect.X, rect.Y, columnWidths[0], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Location", font, XBrushes.Black, new XRect(rect.X + columnWidths[0], rect.Y, columnWidths[1], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Plate License", font, XBrushes.Black, new XRect(rect.X + columnWidths[0] + 90 + columnWidths[1], rect.Y, columnWidths[2], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Check In", font, XBrushes.Black, new XRect(rect.X + columnWidths[0] + 20 + columnWidths[1] + columnWidths[2], rect.Y, columnWidths[3], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Check Out", font, XBrushes.Black, new XRect(rect.X + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3], rect.Y, columnWidths[4], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Parking Fee (RON)", font, XBrushes.Black, new XRect(rect.X + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3] + columnWidths[4], rect.Y, columnWidths[5], rowHeight), XStringFormats.TopLeft);

            currentY += rowHeight;
            int counter = 1;

            foreach (var result in results)
            {
                gfx.DrawString(counter.ToString(), font, XBrushes.Black, new XRect(marginLeft, currentY, columnWidths[0], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(result.Parking.Location, font, XBrushes.Black, new XRect(marginLeft + columnWidths[0], currentY, columnWidths[1], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(result.Vehicle.PlateLicence, font, XBrushes.Black, new XRect(marginLeft + columnWidths[0] + 90 + columnWidths[1], currentY, columnWidths[2], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(result.Vehicle.CheckIn.ToString(), font, XBrushes.Black, new XRect(marginLeft + columnWidths[0] + 20 + columnWidths[1] + columnWidths[2], currentY, columnWidths[3], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(result.Vehicle.CheckOut.ToString(), font, XBrushes.Black, new XRect(marginLeft + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3], currentY, columnWidths[4], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString($"{result.Vehicle.ParkingFee} RON", font, XBrushes.Black, new XRect(marginLeft + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3] + columnWidths[4], currentY, columnWidths[5], rowHeight), XStringFormats.TopLeft);

                currentY += rowHeight;
                counter++;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray();
            }
        }

    }
}
