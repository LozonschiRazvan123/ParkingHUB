using ParkingHUB.DTO;
using ParkingHUB.Pagination;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Interface
{
    public interface IParking
    {
        Task<IEnumerable<ParkingDTO>> GetParkings();
        Task<ParkingDTO> GetParkingsId(int id);
        Task<IEnumerable<ParkingListViewModel>> GetParkingId(int id);
        Task<IEnumerable<ParkingListViewModel>> GetParkingVehicle();
        Task<PageResult<ParkingListViewModel>> GetParkingVehicleInLocation(string location, PaginationPage filter);
        Task<PageResult<ParkingListViewModel>> Search(DateTime checkIn, DateTime checkOut, int pageNumber = 1, int pageSize = 10);
        Task<bool> ExtendParkingTime(int vehicleId, DateTime newEndTime);
        bool CreateParking(ParkingListViewModel parking, string userId);
        bool DeleteParking(int parkingId);
        bool UpdateParkingImage(ParkingDTO parking);
        bool CreateLocationForParking(ParkingDTO parking);
        Task<byte[]> GetImageById(int id);
        Task<byte[]> GenereateVehicleParkingPDF(DateTime checkIn, DateTime checkOut);
    }
}
