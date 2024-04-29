using ParkingHUB.DTO;

namespace ParkingHUB.Interface
{
    public interface IEmail
    {
        void SendEmail(EmailDTO request);
    }
}
