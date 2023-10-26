namespace MenuSystem;

public class GameEngine
{
    private readonly GameSetup _gameSetup;
    private readonly CardDeck _deck;
    private readonly GameSettings _gameSettings;

    public GameEngine(string settingsFilePath)
    {
        var jsonString = File.ReadAllText(settingsFilePath);
        _gameSettings = new GameSettings(jsonString);
        _gameSetup = new GameSetup();
        _deck = new CardDeck(settingsFilePath); 
    }

    public void StartGame()
    {
        // Create and sit players
        string playersInfo = _gameSetup.CreatePlayers();
        List<Player> players = _gameSetup.ParsePlayerInfo(playersInfo);
        players = _gameSetup.SitPlayers(players, _gameSettings);
        _gameSetup.PrintPlayersList(players);
        // Initialize and shuffle the deck
        _deck.InitializeDeck();
        Console.WriteLine($"cards in the deck: {_deck.Cards.Count}");
        _deck.ShuffleDeck();
        _deck.PrintDeck();
        // Deal the cards to players
        var cardDealer = new CardsDeal(_deck, players, _gameSettings.NumberOfCardsPerPlayer);
        cardDealer.DealCards();
        // Randomly select the first player
        _gameSetup.DetermineFirstPlayerAndReorder(players);
        // Draw the random card to start the game
        Card startingCard = _deck.DrawCard("random"); 
    }
}