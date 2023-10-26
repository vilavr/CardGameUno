namespace MenuSystem;

public class GameEngine
{
    private readonly CardDeck _deck;
    private readonly GameSettings _gameSettings;
    private readonly GameSetup _gameSetup;
    private List<Player> _players;

    public GameEngine(string settingsFilePath)
    {
        var jsonString = File.ReadAllText(settingsFilePath);
        _gameSettings = new GameSettings(jsonString);
        _gameSetup = new GameSetup();
        _deck = new CardDeck(settingsFilePath);
        _players = new List<Player>();
    }

    public void StartGame()
    {
        // Create and sit players
        var playersInfo = _gameSetup.CreatePlayers();
        _players = _gameSetup.ParsePlayerInfo(playersInfo);
        _players = _gameSetup.SitPlayers(_players, _gameSettings);
        _gameSetup.PrintPlayersList(_players);
        // Initialize and shuffle the deck
        _deck.InitializeDeck();
        Console.WriteLine($"cards in the deck: {_deck.Cards.Count}");
        _deck.ShuffleDeck();
        _deck.PrintDeck();
        // Deal the cards to players
        var cardDealer = new CardsDeal(_deck, _players, _gameSettings.NumberOfCardsPerPlayer);
        cardDealer.DealCards();
        // Randomly select the first player
        _gameSetup.DetermineFirstPlayerAndReorder(_players);
        // Draw the random card to start the game
        var startingCard = _deck.DrawCard("random");
        // ManualCardManagementTest(); // debugging adding and removing cards
    }

    

    
    
    // Debugging adding and removing cards
    // public Player? GetPlayerById(int playerId)
    // {
    //     // Using LINQ to find the player with the corresponding ID.
    //     // This assumes that your Player class has an 'Id' property that stores the player's ID.
    //     var player = _players.FirstOrDefault(p => p.Id == playerId);
    //
    //     return player;
    // }
    // public void ManualCardManagementTest()
    // {
    //     Console.WriteLine("Manual Card Management Test Mode. Type 'exit' to quit.");
    //
    //     while (true)
    //         try
    //         {
    //             Console.Write("Enter Player ID: ");
    //             var input = Console.ReadLine()!;
    //             if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
    //
    //             if (!int.TryParse(input, out var playerId))
    //             {
    //                 Console.WriteLine("Invalid player ID. Please enter a numeric ID.");
    //                 continue;
    //             }
    //
    //             // You need a method to get the player by ID. This method might belong to another class managing your players.
    //             var player = GetPlayerById(playerId); // This needs to be replaced or implemented.
    //             if (player == null)
    //             {
    //                 Console.WriteLine($"No player found with ID {playerId}.");
    //                 continue;
    //             }
    //
    //             Console.Write("Enter card color: ");
    //             var cardColorStr = Console.ReadLine()!;
    //             // Convert the string to your CardColor enum.
    //             if (!Enum.TryParse(cardColorStr, true, out CardColor cardColor))
    //             {
    //                 Console.WriteLine("Invalid card color. Please enter a correct color.");
    //                 continue;
    //             }
    //
    //             Console.Write("Enter card value: ");
    //             var cardValueStr = Console.ReadLine()!;
    //             // Convert the string to your CardValue enum.
    //             if (!Enum.TryParse(cardValueStr, true, out CardValue cardValue))
    //             {
    //                 Console.WriteLine("Invalid card value. Please enter a correct value.");
    //                 continue;
    //             }
    //
    //             // Create a new Card object using the constructor
    //             var card = new Card(cardColor, cardValue); // Adjusted to use the constructor.
    //
    //             Console.Write("Do you want to 'add' or 'get rid' of the card? (add/get): ");
    //             var operation = Console.ReadLine()!;
    //
    //             var jsonFilePath =
    //                 "/home/viralavrova/cardgameuno/Uno/Resources/players_info.json"; // Change this to your actual JSON file path
    //
    //             if (operation.Equals("add", StringComparison.OrdinalIgnoreCase))
    //             {
    //                 player.TakeCard(card, jsonFilePath);
    //                 Console.WriteLine(
    //                     $"Card {card} added to player {playerId}'s hand."); // Using ToString() from Card class
    //             }
    //             else if (operation.Equals("get", StringComparison.OrdinalIgnoreCase))
    //             {
    //                 player.GetRidOfCard(card, jsonFilePath);
    //                 Console.WriteLine(
    //                     $"Card {card} removed from player {playerId}'s hand."); // Using ToString() from Card class
    //             }
    //             else
    //             {
    //                 Console.WriteLine("Invalid operation. Please type 'add' or 'get'.");
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"An error occurred: {ex.Message}");
    //             // For debugging
    //             Console.WriteLine(ex.StackTrace);
    //         }
    // }
}