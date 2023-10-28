using Newtonsoft.Json;

namespace MenuSystem;

public class GameSaver
{
    private readonly string
        _playersInfoPath = "/home/viralavrova/cardgameuno/Uno/Resources/players_info.json"; // Path to players info

    private readonly string
        _settingsInfoPath = "/home/viralavrova/cardgameuno/Uno/Resources/settings_info.json"; // Path to game settings

    public void SaveGame(GameState gameState)
    {
        Console.WriteLine("Please enter a file name to save the current game state (default: gamestate_info.json):");
        var inputFileName = Console.ReadLine()!.Trim();

        var targetDirectory = "/home/viralavrova/cardgameuno/Uno/Resources/";

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory); 

        var gameStateFilePath = Path.Combine(targetDirectory,
            string.IsNullOrEmpty(inputFileName) ? "gamestate_info.json" : inputFileName);

        var playersInfo = File.ReadAllText(_playersInfoPath);
        var settingsInfo = File.ReadAllText(_settingsInfoPath);

        var gameStateInfo = new
        {
            Players = JsonConvert.DeserializeObject(playersInfo),
            Settings = JsonConvert.DeserializeObject(settingsInfo),
            Game = new
            {
                gameState.AvailableCardsInDeck,
                gameState.CardsInDiscard,
                TopCard = gameState.CurrentTopCard,
                gameState.SpecialCardEffectApplied,
                gameState.CurrentPlayerTurn,
                gameState.CurrentRound
            }
        };

        var jsonToWrite = JsonConvert.SerializeObject(gameStateInfo, Formatting.Indented);

        try
        {
            File.WriteAllText(gameStateFilePath, jsonToWrite);
            Console.WriteLine($"Game state saved successfully to \"{gameStateFilePath}\".");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving the game: {ex.Message}");
        }
    }
}