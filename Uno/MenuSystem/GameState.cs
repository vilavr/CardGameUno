namespace MenuSystem;

public class GameState
{
    public List<Card> AvailableCardsInDeck { get; set; }
    public List<Card> CardsInDiscard { get; set; }
    public bool SpecialCardEffectApplied { get; set; }
    public Card? _currentTopCard; // backing field

    public Card? CurrentTopCard
    {
        get
        {
            // Console.WriteLine("I am in current top card getter");

            // Check if there are any cards in the discard pile and return the last one (the top one).
            if (CardsInDiscard != null && CardsInDiscard.Count > 0)
            {
                return CardsInDiscard
                    [CardsInDiscard.Count - 1]; // The last card in the list is the top of the discard pile.
            }

            return null; // No cards in the discard pile.
        }
        set
        {
            _currentTopCard = value;
            // Console.WriteLine("i am in current top card setter");
            if (CardsInDiscard != null && value != null)
            {
                if (!CardsInDiscard.Contains(value)) // or some other appropriate check
                {
                    CardsInDiscard.Add(value);
                }
            }
        }
    }

    public int CurrentPlayerTurn { get; set; }
    public int CurrentRound { get; set; }
    public List<Player> Players { get; set; }

    public GameState()
    {
        // Initialize the lists to prevent null reference issues.
        AvailableCardsInDeck = new List<Card>();
        CardsInDiscard = new List<Card>();
        Players = new List<Player>();
    }

    public void AddCardToDiscard(Card card)
    {
        CardsInDiscard.Add(card);
    }

    public void PrintGameState(GameState gameState)
    {
        Console.WriteLine("\n--- Current Game State ---");
        Console.WriteLine($"Current Top Card: {gameState.CurrentTopCard}");
        Console.WriteLine($"Current Player's Turn (Player ID): {gameState.CurrentPlayerTurn}");

        // Print the number of cards in the available deck
        Console.WriteLine($"Available Cards In Deck: {gameState.AvailableCardsInDeck.Count}");

        // Optionally, you can uncomment the following lines if you want to display each card in the available deck.
        // foreach (var card in gameState.AvailableCardsInDeck)
        // {
        //     Console.WriteLine(card.ToString()); // Assuming a suitable ToString method in 'Card' class
        // }

        // Print the number of cards in the discard pile
        Console.WriteLine($"Cards In Discard: {gameState.CardsInDiscard.Count}");

        // List each card in the discard pile
        // foreach (var card in gameState.CardsInDiscard)
        // {
        //     Console.WriteLine(card.ToString()); // Assuming a suitable ToString method in 'Card' class
        // }

        Console.WriteLine("Players:");
        foreach (var player in gameState.Players)
        {
            // Print each player's ID, nickname, and the number of cards in their hand
            Console.WriteLine($"Player {player.Id} ({player.Nickname}) - Cards in hand: {player.Hand.Count}");
        }

        Console.WriteLine("--- End of Game State ---\n");
    }
}
