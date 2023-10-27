namespace MenuSystem;

public class Card
{
    public CardColor Color { get; set; }
    public CardValue Value { get; }

    public int Score { get; } // Added Score property

    public Card(CardColor color, CardValue value)
    {
        Color = color;
        Value = value;
        Score = CalculateCardScore(); // Calculate the score during object construction
    }

    // Method to calculate the score of the card based on its value.
    private int CalculateCardScore()
    {
        switch (Value)
        {
            case CardValue.Zero:
                return 0;
            case CardValue.One:
                return 1;
            case CardValue.Two:
                return 2;
            case CardValue.Three:
                return 3;
            case CardValue.Four:
                return 4;
            case CardValue.Five:
                return 5;
            case CardValue.Six:
                return 6;
            case CardValue.Seven:
                return 7;
            case CardValue.Eight:
                return 8;
            case CardValue.Nine:
                return 9;
            case CardValue.Skip:
            case CardValue.Reverse:
            case CardValue.DrawTwo:
                return 20;
            case CardValue.Wild:
            case CardValue.WildDrawFour:
                return 50;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public bool IsSpecialType()
    {
        return Value switch
        {
            CardValue.Skip => true,
            CardValue.Reverse => true,
            CardValue.DrawTwo => true,
            CardValue.Wild => true,
            CardValue.WildDrawFour => true,
            _ => false,
        };
    }
    
    public int? GetNumericValue()
    {
        return IsSpecialType() ? null : (int?)Value;
    }

    public int GetCardValue()
    {
        if (IsSpecialType())
        {
            return Value switch
            {
                CardValue.Wild => 50,
                CardValue.WildDrawFour => 50,
                _ => 20, // Other special cards like Skip, Reverse, DrawTwo
            };
        }
        else
        {
            return GetNumericValue() ?? 0;
        }
    }
    
    public override string ToString()
    {
        return $"{Color} {Value}";
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
