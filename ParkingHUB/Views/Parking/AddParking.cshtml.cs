using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParkingHUB.Views.Parking
{
    public class AddParkingModel : PageModel
    {
        public bool ShowToast { get; set; }

        public void OnGet(DateTime checkIn, DateTime checkOut)
        {
            if ((checkOut - checkIn).TotalHours == 0)
            {
                ShowToast = true;
            }
            else
            {
                ShowToast = false;
            }
        }
    }
}
