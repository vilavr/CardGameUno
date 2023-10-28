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
    
    public static Card DrawCard(List<Card> deck, GameState gameState, string mode = "top")
    {
        if (deck == null || deck.Count == 0)
        {
            // No cards left in the deck. Time to reshuffle the discard pile into the deck.
            if (gameState.CardsInDiscard.Count > 1)
            {
                Card topDiscard = gameState.CardsInDiscard.Last(); // Preserving the top card
                gameState.CardsInDiscard.Remove(topDiscard); 

                deck?.AddRange(gameState.CardsInDiscard);

                // Clearing the discard pile, leaving only the top card
                gameState.CardsInDiscard.Clear();
                gameState.CardsInDiscard.Add(topDiscard); // Putting the top card back

                Random rng = new Random();
                int n = deck!.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Card value = deck[k];
                    deck[k] = deck[n];
                    deck[n] = value;
                }
                Console.WriteLine("Empty deck, reshuffle done");
            }
            else
            {
                throw new InvalidOperationException("No cards available to draw or reshuffle.");
            }
        }

        Card drawnCard;
        if (mode.Equals("random", StringComparison.OrdinalIgnoreCase))
        {
            int index = new Random().Next(deck.Count); // Selecting a random card
            drawnCard = deck[index];
            deck.RemoveAt(index); // Removing the drawn card from the deck
        }
        else
        {
            drawnCard = deck[0]; // Taking the top card
            deck.RemoveAt(0); // Removing the top card from the deck
        }

        return drawnCard;
    }

}
