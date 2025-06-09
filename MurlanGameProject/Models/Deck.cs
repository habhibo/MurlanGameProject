using MuranGameProject.Models;

public class Deck
{
    public string GroupName { get; set; }
    public List<string[]> TheHand { get; set; } = new(); // Changed from Stack
    public List<string[]> OpponentHands { get; set; } = new();
    private readonly Random _rnd = new();
    private readonly Stack<Card> _deck = new();

    public Deck()
    {
    }

    public Deck(string group)
    {
        GroupName = group;
        ShuffleCards();
        FillHands();
    }

    private void ShuffleCards()
    {
        string[] suits = { "spades", "hearts", "clubs", "diamonds" };
        string[] values = { "14", "15", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };
        int id = 0;

        foreach (var suit in suits)
        {
            int r = 1;
            foreach (var value in values)
            {
                _deck.Push(new Card(value, suit, id++, $"rank{r++}"));
            }
        }
        _deck.Push(new Card("16", "joker", id++, "rank1"));
        _deck.Push(new Card("17", "joker", id++, "rank2"));


        var shuffled = _deck.OrderBy(_ => _rnd.Next()).ToArray();
        _deck.Clear();
        foreach (var card in shuffled)
        {
            _deck.Push(card);
        }
    }

    private void FillHands()
    {
        var hands = new List<string>[] { new(), new(), new(), new() };

        while (_deck.Count > 2)
        {
            hands[0].Add(CardToString(_deck.Pop()));
            hands[1].Add(CardToString(_deck.Pop()));
            hands[2].Add(CardToString(_deck.Pop()));
            hands[3].Add(CardToString(_deck.Pop()));
        }
        hands[0].Add(CardToString(_deck.Pop()));
        hands[1].Add(CardToString(_deck.Pop()));

        TheHand.Clear();
        OpponentHands.Clear();
        for (int i = 0; i < 4; i++)
        {
            var sortedHand = hands[i]
                .OrderByDescending(h => int.Parse(h.Split('|')[1])) 
                .ThenByDescending(h => GetSuitRank(h.Split('|')[2])) 
                .ThenByDescending(h => h.Split('|')[2] == "joker" ? (h.Split('|')[3] == "rank3" ? 1 : 0) : 0) 
                .ToArray();
            TheHand.Add(sortedHand);
            OpponentHands.Add(sortedHand);
        }
    }

    // Make GetSuitRank static
    public static int GetSuitRank(string suit)
    {
        switch (suit)
        {
            case "spades": return 4;
            case "hearts": return 3;
            case "diamonds": return 2;
            case "clubs": return 1;
            case "joker": return 5;
            default: return 0;
        }
    }

    private string CardToString(Card card)
    {
        return $"{card.Id}|{card.Value}|{card.Suit}|{card.Rank}";
    }
}