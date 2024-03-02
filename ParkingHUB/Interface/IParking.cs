using ParkingHUB.DTO;

namespace ParkingHUB.Interface
{
    public interface IParking
    {
        Task<IEnumerable<ParkingDTO>> GetParkings();
    }
}
