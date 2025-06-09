using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MuranGameProject.Models;

namespace MuranGameProject.Pages
{
    [Route("/Profile")]
    public class ProfileModel : PageModel
    {
        private readonly Business _business;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(Business business, ILogger<ProfileModel> logger)
        {
            _business = business;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public DateTime RegistrationDate { get; set; }
        [BindProperty]
        public string ProfilePicture { get; set; }
        [BindProperty]
        public string CurrentPassword { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string ConfirmNewPassword { get; set; }
        [BindProperty]
        public string SelectedPicture { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public bool IsFirstLoad { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Login");
            }

            Email = HttpContext.Session.GetString("UserEmail");
            var user = await _business.GetUserInfo(Email);
            if (user != null)
            {
                Name = user.Name;
                Surname = user.Surname;
                RegistrationDate = user.RegistrationDate;
                ProfilePicture = user.ProfilePicture ?? "/images/profile1.png";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePictureAsync()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Login");
            }

            Email = HttpContext.Session.GetString("UserEmail");

            // Check if the profile picture already exists
            var user = await _business.GetUserInfo(Email);
            if (user != null)
            {
                Name = user.Name;
                Surname = user.Surname;
                RegistrationDate = user.RegistrationDate;

                _logger.LogInformation("////////////////////////////////////////////////");
                _logger.LogInformation($"User found - ProfilePicture: {user.ProfilePicture}");
                _logger.LogInformation("////////////////////////////////////////////////");
                ProfilePicture = user.ProfilePicture ?? "/images/profile1.png";

                // Only update if a new picture is actually selected
                if (!string.IsNullOrEmpty(SelectedPicture))
                {
                    // Check if we're updating with the same picture
                    bool isCurrentPicture = user.ProfilePicture == SelectedPicture;

                    // If it's already the current picture, no need to update
                    if (isCurrentPicture)
                    {
                        ProfilePicture = SelectedPicture;
                        _logger.LogInformation("1111111111111111111111111111111111111111111111");

                    }
                    else if (await _business.UpdateProfilePicture(Email, SelectedPicture))
                    {
                        ProfilePicture = SelectedPicture;
                        SuccessMessage = "Profile picture updated successfully!";
                        _logger.LogInformation("222222222222222222222222222222222222222222222222");

                    }
                    else
                    {
                        ProfilePicture = user.ProfilePicture ?? "/images/profile1.png";
                        ErrorMessage = "Failed to update profile picture.";
                        _logger.LogInformation("3333333333333333333333333333333333333333333333333333");

                    }
                }
                else
                {
                    // No picture selected, use existing one
                    ProfilePicture = user.ProfilePicture ?? "/images/profile1.png";
                }
            }

            return Page();
        }

        public async Task<IActionResult> reloadPicturePictureAsync()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Login");
            }

            Email = HttpContext.Session.GetString("UserEmail");

            // Check if the profile picture already exists
            var user = await _business.GetUserInfo(Email);
            if (user != null)
            {
                Name = user.Name;
                Surname = user.Surname;
                RegistrationDate = user.RegistrationDate;

                _logger.LogInformation("////////////////////////////////////////////////");
                _logger.LogInformation($"User found - ProfilePicture: {user.ProfilePicture}");
                _logger.LogInformation("////////////////////////////////////////////////");
                ProfilePicture = user.ProfilePicture ?? "/images/profile1.png";

            }

            return Page();
        }


        public async Task<IActionResult> OnPostChangePasswordAsync()
        {


            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Login");
            }

            Email = HttpContext.Session.GetString("UserEmail");
            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                var user1 = await _business.GetUserInfo(Email);
                Name = user1.Name;
                Surname = user1.Surname;
                RegistrationDate = user1.RegistrationDate;
                ProfilePicture = user1.ProfilePicture ?? "/images/profile1.png";
                return Page();
            }

            if (await _business.ChangePassword(Email, CurrentPassword, NewPassword))
            {
                SuccessMessage = "Password changed successfully!";
            }
            else
            {
                ErrorMessage = "Current password is incorrect or new password must be at least 8 characters.";
            }

            var user = await _business.GetUserInfo(Email);
            Name = user.Name;
            Surname = user.Surname;
            RegistrationDate = user.RegistrationDate;

            ProfilePicture = user.ProfilePicture ?? "/images/profile1.png";
            return Page();
        }
    }
}