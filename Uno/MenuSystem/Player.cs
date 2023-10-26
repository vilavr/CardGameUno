namespace MenuSystem;

public class Player
{
    public int Id { get; }
    public string Nickname { get; set; }
    public EPlayerType Type { get; set; }

    // This list holds the cards that the player has.
    public List<Card> Hand { get; }

    public Player(int id, string nickname, EPlayerType type)
    {
        Id = id;
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
        Type = type;
        Hand = new List<Card>();
    }
    public void ReceiveCard(Card card)
    {
        if (card == null)
        {
            throw new ArgumentNullException(nameof(card), "Cannot add a null card to a player's hand.");
        }

        Hand.Add(card);
    }
    public IReadOnlyList<Card> ShowHand()
    {
        return Hand.AsReadOnly();
    }
}
