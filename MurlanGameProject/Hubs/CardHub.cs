using Microsoft.AspNetCore.SignalR;
using MuranGameProject.Services;

namespace MuranGameProject.Hubs
{
    public class CardHub : Hub
    {
        private readonly GameService _gameService;

        public CardHub(GameService gameService)
        {
            _gameService = gameService;
        }

        public async Task GetMyHand()
        {
            var (userGroup, deck) = _gameService.GetUserHand(Context.ConnectionId);
            if (userGroup == null || deck == null) return;

            bool turn = userGroup.Hand.Any(card => card.Contains("spades") && card.Contains("rank3"));
            var otherPlayers = deck.OpponentHands.Where(h => !h.SequenceEqual(userGroup.Hand)).ToList();

            await Clients.Client(userGroup.ConnId).SendAsync("recieveHand", userGroup.ConnId, userGroup.Hand, turn);
            await Clients.Client(userGroup.ConnId).SendAsync("p1Hand", userGroup.ConnId, otherPlayers[0]);
            await Clients.Client(userGroup.ConnId).SendAsync("p2Hand", userGroup.ConnId, otherPlayers[1]);
            await Clients.Client(userGroup.ConnId).SendAsync("p3Hand", userGroup.ConnId, otherPlayers[2]);
        }

        public async Task JoinRoom(string roomName)
        {
            if (_gameService.AddUserToGroup(roomName, Context.ConnectionId, _gameService.SomeUserConnId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            }
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task BroadcastPlay(string[] play)
        {
            bool freeHand = play.Length == 0 ? ++_gameService.UserPassed == _gameService.PlayersIngame : (_gameService.UserPassed = 0) == 0;

            await Clients.Client(Context.ConnectionId).SendAsync("enablePlay", false);
            var nextPlayer = _gameService.Groups.First(ug => ug.ConnId == Context.ConnectionId).NextPlayerConnId;
            await Clients.Client(nextPlayer).SendAsync("enablePlay", true);
            await Clients.Group("8080").SendAsync("recievePlay", Context.User?.Identity?.Name, play, freeHand);
        }

        public async Task PlayerIsOut()
        {
            _gameService.PlayersIngame--;
            var group = _gameService.Groups.First(ug => ug.ConnId == Context.ConnectionId).Group;

            if (_gameService.PlayersIngame == 0)
            {
                var lastPlayer = _gameService.Groups.First(ug => ug.ConnId == Context.ConnectionId);
                lastPlayer.Place = 4;
                var placements = string.Join("|", _gameService.Groups.Where(ug => ug.Group == group).Select(ug => $"{ug.Place} {ug.Username}"));
                await Clients.Group("8080").SendAsync("allOut", placements);
            }
            else
            {
                var currentPlayer = _gameService.Groups.First(ug => ug.ConnId == Context.ConnectionId);
                currentPlayer.Place = ++_gameService.Place;
                var nextPlayer = currentPlayer.NextPlayerConnId;
                await Clients.Client(nextPlayer).SendAsync("enablePlay", true);

                var prevPlayer = _gameService.Groups.First(ug => ug.NextPlayerConnId == Context.ConnectionId);
                prevPlayer.NextPlayerConnId = nextPlayer;
            }
        }

        public async Task CallTheSpread()
        {
            if (_gameService.Groups.Count(ug => ug.Group == "8080") == 4)
            {
                await Clients.Group("8080").SendAsync("spreadCards");
            }
        }
    }
}