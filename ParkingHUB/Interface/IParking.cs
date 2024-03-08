using ParkingHUB.DTO;
using ParkingHUB.Pagination;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Interface
{
    public interface IParking
    {
        Task<IEnumerable<ParkingDTO>> GetParkings();
        Task<IEnumerable<ParkingListViewModel>> GetParkingId(int id);
        Task<IEnumerable<ParkingListViewModel>> GetParkingVehicle();
        Task<PageResult<ParkingListViewModel>> GetParkingVehicleInLocation(string location, PaginationPage filter);
        bool CreateParking(ParkingListViewModel parking);
        bool DeleteParking(int parkingId);
    }
}
