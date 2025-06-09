using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MuranGameProject.Pages
{
    public class GameModeModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                Response.Redirect("/Login");
            }
        }
    }
}