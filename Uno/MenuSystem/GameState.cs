namespace MenuSystem;

public class GameState
{
    public List<Card> AvailableCardsInDeck { get; set; }
    public List<Card> CardsInDiscard { get; set; }
    public bool SpecialCardEffectApplied { get; set; }
    public bool IsSaveInitiated { get; set; } = false;
    public Card? CurrentTopCard 
    { 
        get 
        {
            return CardsInDiscard?.LastOrDefault();
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
        // Console.WriteLine("Available Cards In Deck:");
        // foreach (var card in gameState.AvailableCardsInDeck)
        // {
        //     Console.WriteLine(card.ToString()); // Assuming a suitable ToString method in 'Card' class
        // }
        // Console.WriteLine("Cards In Discard:");
        // foreach (var card in gameState.CardsInDiscard)
        // {
        //     Console.WriteLine(card.ToString()); // Same assumption as above
        // }
        Console.WriteLine("Players:");
        foreach (var player in gameState.Players)
        {
            Console.WriteLine($"Player {player.Id}: {player.Nickname}"); // Assuming 'Id' and 'Name' properties
        }
        Console.WriteLine("--- End of Game State ---\n");
    }
}