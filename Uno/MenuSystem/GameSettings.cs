using System.Text.Json;

namespace MenuSystem;

public class GameSettings
{
    public int NumberOfDecks { get; private set; }
    public int NumberOfCardsPerPlayer { get; private set; }
    public int WinningScore { get; private set; }
    public string PlayDirection { get; private set; } = default!;

    public GameSettings(string jsonConfig)
    {
        ParseGameSettingsFromJson(jsonConfig);
    }

    private void ParseGameSettingsFromJson(string jsonConfig)
    {
        try
        {
            using (JsonDocument document = JsonDocument.Parse(jsonConfig))
            {
                JsonElement root = document.RootElement;
                JsonElement gameSettings = root.GetProperty("gameSettings");

                NumberOfDecks = gameSettings.GetProperty("NumberOfDecks").GetInt32();
                NumberOfCardsPerPlayer = gameSettings.GetProperty("NumberOfCardsPerPlayer").GetInt32();
                WinningScore = gameSettings.GetProperty("WinningScore").GetInt32();
                PlayDirection = gameSettings.GetProperty("PlayDirection").GetString();
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format for game settings.", ex);
        }
    }
}
