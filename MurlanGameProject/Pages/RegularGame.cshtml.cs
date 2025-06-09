using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MuranGameProject.Models;
using Newtonsoft.Json;

namespace MuranGameProject.Pages
{
    public class RegularGameModel : PageModel
    {

        private readonly Business _business;
        private readonly ILogger<RegularGameModel> _logger;


        public RegularGameModel(Business business, ILogger<RegularGameModel> logger)
        {
            _business = business;
            _logger = logger;
        }

        public string UserProfilePicture { get; set; } = "/images/profile1.png"; // Default value

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Login");
            }

            // Fetch user info to get profile picture
            var email = HttpContext.Session.GetString("UserEmail");
            var user = await _business.GetUserInfo(email);
            _logger.LogInformation("////////////////////////////////////////////////");
            _logger.LogInformation($"User found - ProfilePicture: {user.ProfilePicture}");
            _logger.LogInformation("////////////////////////////////////////////////");

            // Set profile picture with null safety
            if (user != null && !string.IsNullOrEmpty(user.ProfilePicture))
            {
                UserProfilePicture = user.ProfilePicture;
            }

            // Log the profile picture URL for debugging
            Console.WriteLine($"Profile picture URL: {UserProfilePicture}");

            if (!HttpContext.Session.TryGetValue("SinglePlayerDeck", out _))
            {
                var deck = new Deck("SinglePlayer");
                HttpContext.Session.Set("SinglePlayerDeck", System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deck)));
            }

            return Page();
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                Response.Redirect("/Login");
            }
            if (!HttpContext.Session.TryGetValue("SinglePlayerDeck", out _))
            {
                var deck = new Deck("SinglePlayer");
                HttpContext.Session.Set("SinglePlayerDeck", System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deck)));
            }
        }
        public IActionResult OnPostResetDeck()
        {
            var deck = new Deck("SinglePlayer");
            HttpContext.Session.Set("SinglePlayerDeck", System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deck)));
            return new OkResult();
        }

        public async Task<IActionResult> OnPostGetInitialHandsAsync()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Login");
            }

            _logger.LogInformation("Fetching initial hands...");

            // Fetch user info to get profile picture
            var email = HttpContext.Session.GetString("UserEmail");
            var user = await _business.GetUserInfo(email);
            _logger.LogInformation("////////////////////////////////////////////////");
            _logger.LogInformation($"User found - ProfilePicture: {user.ProfilePicture}");
            _logger.LogInformation("////////////////////////////////////////////////");

            // Set profile picture with null safety
            string profilePicture = "/images/profile1.png"; // Default
            if (user != null && !string.IsNullOrEmpty(user.ProfilePicture))
            {
                profilePicture = user.ProfilePicture;
                _logger.LogInformation("Profile picture set to: " + profilePicture);
            }

            var deckBytes = HttpContext.Session.Get("SinglePlayerDeck");
            if (deckBytes == null)
            {
                var deck1 = new Deck("SinglePlayer");
                HttpContext.Session.Set("SinglePlayerDeck", System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deck1)));
                deckBytes = HttpContext.Session.Get("SinglePlayerDeck");
            }

            var deck = JsonConvert.DeserializeObject<Deck>(System.Text.Encoding.UTF8.GetString(deckBytes));

            for (int i = 0; i < deck.TheHand.Count; i++)
            {
                deck.TheHand[i] = deck.TheHand[i]
                    .OrderByDescending(h => int.Parse(h.Split('|')[1])).ThenByDescending(h => h.Split('|')[2] == "joker" ? (h.Split('|')[3] == "rank3" ? 1 : 0) : 0) // Tertiary: Red Joker (rank3) before Black Joker (rank1)
                    .ToArray();
            }

            var hands = new List<string[]>
    {
        deck.TheHand[0], // Player4
        deck.TheHand[1], // Player3
        deck.TheHand[2], // Player2
        deck.TheHand[3]  // Player1
    };

                        // Return hands and profile picture
            return new JsonResult(new
            {
                Hands = hands,
                ProfilePicture = profilePicture
            });
        }
    }
}