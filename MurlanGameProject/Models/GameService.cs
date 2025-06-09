using MuranGameProject.Models;

namespace MuranGameProject.Services
{
    public class GameService
    {
        public List<UserGroup> Groups { get; } = new();
        public List<Deck> Decks { get; } = new();
        public string? UserEmail { get; set; }
        public int UserId { get; set; }
        public string? SomeUserConnId { get; set; }
        public string? FirstPlayerConnId { get; set; }
        public int UserPassed { get; set; }
        public int PlayersIngame { get; set; } = 3;
        public int Place { get; set; } = 1;

        public (UserGroup?, Deck?) GetUserHand(string connId)
        {
            var userGroup = Groups.FirstOrDefault(ug => ug.ConnId == connId);
            var deck = Decks.FirstOrDefault(d => d.GroupName == userGroup?.Group);
            return (userGroup, deck);
        }

        public bool AddUserToGroup(string groupName, string connId, string? prevConnId = null)
        {
            var groupMembers = Groups.Count(ug => ug.Group == groupName);
            if (groupMembers >= 4) return false;

            if (groupMembers == 0)
            {
                Decks.Add(new Deck(groupName));
                Groups.Add(new UserGroup { Group = groupName, ConnId = connId });
                SomeUserConnId = connId;
                FirstPlayerConnId = connId;
            }
            else if (groupMembers < 3)
            {
                Groups.Add(new UserGroup { Group = groupName, ConnId = connId, NextPlayerConnId = prevConnId });
                SomeUserConnId = connId;
            }
            else
            {
                var firstPlayer = Groups.First(ug => ug.ConnId == FirstPlayerConnId);
                firstPlayer.NextPlayerConnId = connId;
                Groups.Add(new UserGroup { Group = groupName, ConnId = connId, NextPlayerConnId = prevConnId });
            }
            return true;
        }
    }
}
