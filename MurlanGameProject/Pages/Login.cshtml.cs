using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MuranGameProject.Models;

namespace MuranGameProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Business _business;

        public LoginModel(Business business)
        {
            _business = business;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _business.Authenticate(Email, Password))
            {
                HttpContext.Session.SetString("UserEmail", Email);
                return RedirectToPage("/GameMode");
            }
            return Page();
        }
    }
}