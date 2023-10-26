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

        // Instantiate GameState and set the initial deck
        GameState gameState = new GameState();
        gameState.AvailableCardsInDeck = new List<Card>(_deck.Cards);

        // Deal the cards to players
        var cardDealer = new CardsDeal(_deck, _players, _gameSettings.NumberOfCardsPerPlayer);
        cardDealer.DealCards();

        // Randomly select the first player and set it in the game state
        _gameSetup.DetermineFirstPlayerAndReorder(_players);
        gameState.CurrentPlayerTurn = _players[0].Id; // Assuming Id is a property of the player

        // Draw the random card to start the game and set it in the game state
        Card startingCard = _deck.DrawCard("random");
        gameState.SetInitialTopCard(startingCard);

        // Initialize the discard pile with the starting card
        gameState.CardsInDiscard = new List<Card> { startingCard };


        // For debugging or testing, you can enable the below methods:
        // ManualCardManagementTest(); // Debugging adding and removing cards
        // TestTurnManager();
    }


    
    // public void TestTurnManager()
    // {
    //     Console.WriteLine("\n==== Testing Turn Manager ====");
    //
    //     // No need to create a new TurnManager instance; we're working directly with the GameEngine methods.
    //
    //     while (true)
    //     {
    //         // Display the current player's ID.
    //         int currentId = _gameSetup.GetCurrentPlayerId(_players);
    //         Console.WriteLine($"Current player's turn: Player {currentId}");
    //         Console.WriteLine("Options: 'advance' to advance turn, 'reverse' to reverse player order, 'show' to show current player ID, 'exit' to end test.");
    //         Console.Write("Enter your choice: ");
    //         var choice = Console.ReadLine()?.ToLower();
    //
    //         switch (choice)
    //         {
    //             case "advance":
    //                 _gameSetup.AdvanceTurn(_players);  // Now calling the GameEngine's method.
    //                 Console.WriteLine("Advanced to the next player's turn.");
    //                 break;
    //
    //             case "reverse":
    //                 _gameSetup.ReversePlayerOrder(_players);  // Now calling the GameEngine's method.
    //                 Console.WriteLine("Reversed the player order.");
    //                 break;
    //
    //             case "show":
    //                 // The current player is still the same; no changes were made in this case.
    //                 Console.WriteLine($"Current player's turn: Player {currentId}");
    //                 break;
    //
    //             case "exit":
    //                 Console.WriteLine("Exiting Turn Manager test.");
    //                 return;
    //
    //             default:
    //                 Console.WriteLine("Invalid choice. Try again.");
    //                 break;
    //         }
    //
    //         // Display the updated players list after each operation.
    //         Console.WriteLine("Updated players order:");
    //         _gameSetup.PrintPlayersList(_players);  // Assuming PrintPlayersList is a method from GameSetup or similar functionality within GameEngine.
    //     }
    // }


    
    
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