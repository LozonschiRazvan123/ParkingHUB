using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Repository;

namespace ParkingHUB.Controllers
{
    public class EmailController:Controller
    {
        private readonly IEmail _emailService;
        private readonly UserManager<User> _userManager;

        public EmailController(IEmail emailService, UserManager<User> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailDTO request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            request.From = currentUser.Email;
            request.Subject = "Feedback from ParkingHUB";

            _emailService.SendEmail(request);

            return Ok();
        }
    }
}
