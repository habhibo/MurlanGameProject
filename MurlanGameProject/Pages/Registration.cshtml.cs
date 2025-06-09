using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MuranGameProject.Models;

namespace MuranGameProject.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly Business _business;

        public RegistrationModel(Business business)
        {
            _business = business;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _business.Register(Email, Name, Surname, Password, ConfirmPassword))
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }
    }
}