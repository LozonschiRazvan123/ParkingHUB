using System.ComponentModel.DataAnnotations;

namespace ParkingHUB.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "The email field is required!")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid email address")]
        public string EmailAddress { get; set; }
    }
}
