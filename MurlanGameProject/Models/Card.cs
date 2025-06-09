namespace MuranGameProject.Models
{
    public class Card
    {
        public string Rank { get; }
        public string Value { get; }
        public string Suit { get; }
        public int Id { get; }

        public Card(string value, string suit, int id, string rank)
        {
            Value = value;
            Id = id;
            Suit = suit;
            Rank = rank;
        }
    }
}