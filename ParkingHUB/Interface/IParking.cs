using ParkingHUB.DTO;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Interface
{
    public interface IParking
    {
        Task<IEnumerable<ParkingDTO>> GetParkings();
        Task<IEnumerable<ParkingListViewModel>> GetParkingVehicle();
    }
}
