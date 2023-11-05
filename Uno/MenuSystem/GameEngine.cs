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

    private bool _didUserSaveGame;

    public void StartGame(bool didUserSaveGame = false, string? savedGameFilePath = null)
    {
        GameState gameState = new GameState();
        bool gameIsRunning = true;

        _didUserSaveGame = didUserSaveGame;
        
        while (gameIsRunning)
        {
            // Console.WriteLine($"id after exiting from previous round but before initialize game: {gameState.CurrentPlayerTurn}");
            InitializeGame(ref gameState, didUserSaveGame, savedGameFilePath);
            gameState.PrintGameState(gameState);
            // Console.WriteLine($"id after exiting from previous round and initialize game: {gameState.CurrentPlayerTurn}");
            bool exitFlag = StartRound(ref gameState);

            if (exitFlag) gameIsRunning = false;
            // Check for the end of the game based on your conditions (e.g., player reaching a specific score)
            var winningPlayer = _players.FirstOrDefault(player => player.Score >= _gameSettings.WinningScore);

            if (winningPlayer != null)
            {
                // Announce the winner and set the flag to end the game
                Console.WriteLine(
                    $"{winningPlayer.Nickname} has reached the winning score of {_gameSettings.WinningScore}!");
                EndGame(winningPlayer);
                gameIsRunning = false; // This will cause the game loop to exit
            }
            else
            {
                // If no winner, the game continues to the next round
                _currentRound++;
                Console.WriteLine($"Round {_currentRound} completed. Starting next round...");
                // Optional: You might need additional setup or shuffling between rounds
            }
        }

        // Once the game is no longer running, we can perform any necessary cleanup or end-of-game processes.

    }

    private void EndGame(Player winningPlayer)
    {
        Console.Clear();
        Console.WriteLine("===================================");
        Console.WriteLine("           GAME OVER               ");
        Console.WriteLine("===================================");

        Console.WriteLine($"\nCongratulations, {winningPlayer.Nickname}! You won the game!\n");

        Console.WriteLine($"Winning Score: {winningPlayer.Score}\n");

        Console.WriteLine("Final scores:");
        foreach (var player in _players)
        {
            Console.WriteLine($"{player.Nickname}: {player.Score}");
        }

        Console.WriteLine($"\nTotal rounds played: {_currentRound}");

        Console.WriteLine("\nThank you for playing! We hope to see you again soon.");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();

        // If the game is part of a larger application and doesn't close, you might return to a main menu instead.
    }

    public void InitializeGame(ref GameState gameState, bool didUserSaveGame = false, string? savedGameFilePath = null)
{
    GameSaver gameSaver = new GameSaver();
    bool isLoaded = false;
    bool isNewRound = false; // Flag to check if it's a new round.
    Console.WriteLine($"_didUSerSaveGame: {_didUserSaveGame}");
    if (_didUserSaveGame)
    {
        // If the game was saved, we load the saved game state.
        gameState = gameSaver.LoadGame(savedGameFilePath)!;
        _players = gameState.Players;
        Console.WriteLine("Game loaded. Resuming from saved state...");
        isLoaded = true; // The game state has been loaded.
        _didUserSaveGame = false;
    }

    if (!isLoaded)
    {
        _currentRound++;
        isNewRound = true;
        gameState.CurrentRound = _currentRound;
    }
    Console.WriteLine($"current round: {_currentRound}");
    if (_currentRound == 1 || isNewRound)
    {
        if (_currentRound == 1)
        {
            _players = _gameSetup.CreatePlayers();
            _players = _gameSetup.SitPlayers(_players, _gameSettings);
            _gameSetup.PrintPlayersList(_players);
            gameState.Players = _players;

            _gameSetup.DetermineFirstPlayerAndReorder(_players);
            gameState.CurrentPlayerTurn = _players[0].Id;
        }

        // Common initialization 
        _deck.Cards.Clear();
        _deck.InitializeDeck();
        _deck.ShuffleDeck();

        foreach (var player in _players)
        {
            player.Hand.Clear(); // Clear players' hands before dealing new cards.
        }

        // Re-deal the cards to players.
        var cardDealer = new CardsDeal(_deck, _players, _gameSettings.NumberOfCardsPerPlayer);
        cardDealer.DealCards();
        _gameSetup.SavePlayersToJson(_players);

        // Clear the previous round's data.
        gameState.AvailableCardsInDeck.Clear();
        gameState.CardsInDiscard.Clear();

        // Update the available cards with the newly initialized deck.
        gameState.AvailableCardsInDeck = new List<Card>(_deck.Cards);

        // Draw the initial card and add it to the discard pile.
        var startingCard = _deck.DrawCard("random");
        gameState.AddCardToDiscard(startingCard);
    }

    if (isLoaded)
    {
        _currentRound++;
    }

    // gameState.PrintGameState(gameState);
}


    public bool StartRound(ref GameState gameState)
    {
        bool exitFlag = false;

        while (true)
        {
            // gameState.PrintGameState(gameState);

            var currentPlayerId = gameState.CurrentPlayerTurn;
            var currentPlayer = _players.FirstOrDefault(p => p.Id == currentPlayerId);

            if (currentPlayer == null)
            {
                Console.WriteLine("[ERROR] No matching player found for the current turn. This should never happen.");
                break;
            }

            // Console.WriteLine($"It's {currentPlayerId}'s turn");

            var playerTurn = new PlayerAction(currentPlayer, gameState);
            var actionResult = playerTurn.TakeTurn();

            if (actionResult == "s")
            {
                exitFlag = true;
                break;
            }

            // Check for victory condition after a player's turn.
            if (currentPlayer.Hand.Count == 0)
            {
                Console.WriteLine($"{currentPlayer.Nickname} has won the round!");
                
                break;
            }

            if (!gameState.SpecialCardEffectApplied)
            {
                _gameSetup.AdvanceTurn(_players, gameState);
            }
            else
            {
                gameState.SpecialCardEffectApplied = false;
            }
        }
        
        return exitFlag;
    }

}