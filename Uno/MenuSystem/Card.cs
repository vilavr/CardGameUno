namespace MenuSystem;

public class Card
{
    public CardColor Color { get; }
    public CardValue Value { get; }

    public Card(CardColor color, CardValue value)
    {
        Color = color;
        Value = value;
    }

    public override string ToString()
    {
        // This will return a string in the format "Color Value", e.g., "Red Three"
        return $"{Color} {Value}";
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Card card)
        {
            return Color == card.Color && Value == card.Value;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Color.GetHashCode() ^ Value.GetHashCode();
    }
    
    public static Card DrawCard(List<Card> deck, string mode = "top")
    {
        if (deck == null || deck.Count == 0)
        {
            throw new InvalidOperationException("Cannot draw a card from an empty deck.");
        }

        Card drawnCard;
        if (mode.Equals("random", StringComparison.OrdinalIgnoreCase))
        {
            int index = new Random().Next(deck.Count); // Assuming you have a random number generator for the index
            drawnCard = deck[index];
            deck.RemoveAt(index); // Removing the drawn card from the deck
        }
        else
        {
            drawnCard = deck[0]; // Top card
            deck.RemoveAt(0); // Removing the top card from the deck
        }

        return drawnCard;
    }
}
