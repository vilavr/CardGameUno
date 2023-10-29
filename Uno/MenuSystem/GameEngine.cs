namespace MenuSystem;

public class GameEngine
{
    private readonly CardDeck _deck;
    private readonly GameSettings _gameSettings;
    private readonly GameSetup _gameSetup;
    private int _currentRound;
    private List<Player> _players;

    public GameEngine(string settingsFilePath)
    {
        var jsonString = File.ReadAllText(settingsFilePath);
        _gameSettings = new GameSettings(jsonString);
        _gameSetup = new GameSetup();
        _deck = new CardDeck(settingsFilePath);
        _players = new List<Player>();
        _currentRound = 0;
    }

    public void StartGame()
    {
        var gameState = new GameState();
        var gameIsRunning = true;
        while (gameIsRunning)
        {
            foreach (var player in gameState.Players)
                if (player.Score >= _gameSettings.WinningScore)
                {
                    Console.WriteLine(
                        $"{player.Nickname} has reached the winning score of {_gameSettings.WinningScore}!");
                    gameIsRunning = false;
                    break;
                }

            InitializeGame(gameState);
            // The game continues until one player reaches the winning score
            var shouldContinue = StartRound(gameState);
            if (!shouldContinue) gameIsRunning = false; // End the game if StartRound signaled to stop.
            foreach (var player in gameState.Players)
            {
                if (player.Score >= _gameSettings.WinningScore)
                {
                    Console.WriteLine($"{player.Nickname} has reached the winning score!");
                    gameIsRunning = false;
                    Console.WriteLine("GAME OVER");

                    var winner = player;
                    // Printing the total number of rounds
                    Console.WriteLine($"Total rounds played: {_currentRound}");

                    // Creating a sorted list of players based on their scores
                    var sortedPlayers = gameState.Players.OrderByDescending(p => p.Score).ToList();

                    // Printing each player's position, nickname, and score
                    var cheeringStatements = new List<string>
                    {
                        "Great effort!",
                        "So close!",
                        "Well played!",
                        "You're amazing!",
                        "Fantastic game!",
                        "Keep it up!",
                        "You were awesome!"
                    };

                    var random = new Random(); // Random number generator

                    for (var i = 0; i < sortedPlayers.Count; i++)
                    {
                        var message = $"{i + 1}. {sortedPlayers[i].Nickname}, {sortedPlayers[i].Score}";
                        if (sortedPlayers[i] == winner)
                        {
                            message += ". Congratulations, you are the winner!!";
                        }
                        else
                        {
                            // If the player didn't win, add a random cheering statement
                            var randomIndex = random.Next(cheeringStatements.Count); // Get a random index
                            message +=
                                $" - {cheeringStatements[randomIndex]}"; // Append a random statement from the list
                        }

                        Console.WriteLine(message);
                    }

                    break;
                }

                if (gameIsRunning)
                {
                    _currentRound++;
                    Console.WriteLine($"Round {_currentRound} completed. Starting next round...");
                }
            }

            
        }
    }

    public void InitializeGame(GameState gameState)
    {
        // GameSaver gameSaver = new GameSaver();
        // string selectedFilePath;
        // bool didUserSaveGame = gameSaver.PromptUserForLoad(out selectedFilePath);
        bool didUserSaveGame = false;
        if (!didUserSaveGame)
        {


            _currentRound++;
            gameState.CurrentRound = _currentRound;
            if (_currentRound == 1)
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
                // _deck.PrintDeck();

                // Instantiate GameState and set the initial deck
                gameState.AvailableCardsInDeck = new List<Card>(_deck.Cards);

                // Deal the cards to players
                var cardDealer = new CardsDeal(_deck, _players, _gameSettings.NumberOfCardsPerPlayer);
                cardDealer.DealCards();

                // Randomly select the first player and set it in the game state
                _gameSetup.DetermineFirstPlayerAndReorder(_players);
                gameState.CurrentPlayerTurn = _players[0].Id; // Assuming Id is a property of the player
                gameState.Players = _players;
                // Draw the random card to start the game and set it in the game state
                var startingCard = _deck.DrawCard("random");

                // Add the starting card to the discard pile, which automatically sets it as the top card
                gameState.AddCardToDiscard(startingCard);
            }
            else
            {
                // Clear the available cards and discard pile in the game state
                gameState.AvailableCardsInDeck.Clear();
                gameState.CardsInDiscard.Clear();
                _deck.Cards.Clear();
                _deck.InitializeDeck();
                foreach (var player in _players)
                    player.Hand.Clear();
                _deck.Cards.AddRange(gameState.CardsInDiscard);
                
                _deck.ShuffleDeck();

                gameState.AvailableCardsInDeck = new List<Card>(_deck.Cards);

                // Output for debugging
                // Console.WriteLine($"cards in the deck: {_deck.Cards.Count}");

                // Deal the cards to players
                var cardDealer = new CardsDeal(_deck, _players, _gameSettings.NumberOfCardsPerPlayer);
                cardDealer.DealCards();

                // foreach (var player in _players) Console.WriteLine($"Dealt cards in player's hand {player.Hand.Count}");

                _gameSetup.SavePlayersToJson(_players);

                var startingCard = _deck.DrawCard("random");

                gameState.AddCardToDiscard(startingCard);
            }
        }
        // else
        // {
        //     gameState = gameSaver.LoadGame(selectedFilePath)!;
        //     Console.WriteLine("Game state in game engine: ");
        //     gameState.PrintGameState(gameState);
        //     _deck.InitializeDeck();
        //     _players.Clear();
        //     _players = gameState.Players;
        //     Console.WriteLine("Game loaded. Resuming from saved state...");
        // }
    }

    public bool StartRound(GameState gameState)
    {
        var gameIsRunning = true;
        gameState.PrintGameState(gameState);
        while (gameIsRunning)
        {
            var playersSnapshot = new List<Player>(_players);

            for (var i = 0; i < playersSnapshot.Count; i++)
            {
                // We need to find the player whose turn it is based on the CurrentPlayerTurn ID
                // Console.WriteLine($"needed id: {gameState.CurrentPlayerTurn}");
                var currentPlayer = _players.FirstOrDefault(p => p.Id == gameState.CurrentPlayerTurn);
                if (currentPlayer == null)
                {
                    Console.WriteLine(
                        "[ERROR] No matching player found for the current turn. This should never happen.");
                    gameIsRunning = false;
                    break;
                }

                // The game only sets the CurrentPlayerTurn if no special card effect (like Skip) was applied in the previous turn.
                if (!gameState.SpecialCardEffectApplied)
                {
                    gameState.CurrentPlayerTurn = currentPlayer.Id;
                }
                else
                {
                    // Console.WriteLine($"[DEBUG] Skip effect active. Skipping Player ID : {currentPlayer.Id}'s turn.");
                    gameState.SpecialCardEffectApplied = false; // Reset the flag after respecting the Skip effect.
                    _gameSetup.AdvanceTurn(_players, gameState);
                    continue;
                }

                // Console.WriteLine($"Id after exiting the if: {currentPlayer.Id}");
                var playerTurn = new PlayerAction(currentPlayer, gameState);
                gameState.PrintGameState(gameState);
                // Console.WriteLine($"Id before taking turn: {currentPlayer.Id}");
                if (playerTurn.TakeTurn() == "s")
                {
                    gameIsRunning = false;
                    break;
                }

                ;

                // Check for victory condition
                if (currentPlayer.Hand.Count == 0)
                {
                    // Console.WriteLine($"{currentPlayer.Nickname} has won the game!");
                    gameIsRunning = false;
                    break;
                }
                if (gameState.SpecialCardEffectApplied)
                {
                    // Console.WriteLine($"[DEBUG] engine Special card effect was applied during Player {currentPlayer.Id}'s turn. Not advancing turn.");
                    // Important: Reset the flag for the next player's turn.
                    gameState.SpecialCardEffectApplied = false;
                }
                else
                {
                    // Console.WriteLine($"[DEBUG] engine No special card effects were applied. Advancing turn.");

                    _gameSetup.AdvanceTurn(_players, gameState);

                    var nextPlayerIndex = (i + 1) % _players.Count;
                    // Console.WriteLine($"[DEBUG] engine Turn advanced. It's now Player {_players[nextPlayerIndex].Id}'s turn.");

                    // Update the gameState's CurrentPlayerTurn to the next player.
                    // gameState.CurrentPlayerTurn = _players[nextPlayerIndex].Id;
                }
            }
        }


        // ManualCardManagementTest(gameState); // Debugging adding and removing cards
        // TestTurnManager(); // Debugging players' turns 
        gameState.PrintGameState(gameState);
        return gameIsRunning;
    }
}