using DAL;
using Domain.Database;

namespace DBoperations;

public class GameSettingsService
{
    private readonly AppDbContext _context;

    public GameSettingsService(AppDbContext context)
    {
        _context = context;
    }

    public int CreateNewGame(string? settingsFileName)
    {
        var game = new Game { SettingsFileName = settingsFileName };
        _context.Games.Add(game);
        _context.SaveChanges();

        // Return the new game's ID
        return game.Id;
    }

    public void EnsureSettingsForGame(int gameId, string settingsFileName)
    {
        // Check if settings for the specified file name already exist
        var settingsExist = _context.GameSettings.Any(gs => gs.FileName == settingsFileName);

        if (!settingsExist)
        {
            // Copy all default settings to the new settings file name
            var defaultSettings = _context.GameSettings.Where(gs => gs.FileName == "default").ToList();
            foreach (var setting in defaultSettings)
            {
                _context.GameSettings.Add(new GameSetting
                {
                    GameId = gameId,
                    FileName = settingsFileName,
                    SettingName = setting.SettingName,
                    SettingValue = setting.SettingValue
                });
            }
            _context.SaveChanges();
        }
        else
        {
            // Update the existing game settings to link with the new game
            var existingSettings = _context.GameSettings.Where(gs => gs.FileName == settingsFileName).ToList();
            foreach (var setting in existingSettings)
            {
                setting.GameId = gameId;
            }
            _context.SaveChanges();
        }
    }

    public IEnumerable<string> GetUniqueSettingFileNames()
    {
        return _context.GameSettings.Select(gs => gs.FileName).Distinct().ToList();
    }

    public void ApplyPreSavedSettings(string settingsFileName, int gameId)
    {
         var existingSettingsForGame = _context.GameSettings
        .Any(gs => gs.FileName == settingsFileName && gs.GameId == gameId);
         if (!existingSettingsForGame)
         {
             var preSavedSettings = _context.GameSettings.Where(gs => gs.FileName == settingsFileName).ToList();
             foreach (var setting in preSavedSettings)
             {
                 // Clone the setting for the new game
                 _context.GameSettings.Add(new GameSetting
                 {
                     GameId = gameId,
                     FileName = settingsFileName,
                     SettingName = setting.SettingName,
                     SettingValue = setting.SettingValue
                 });
             }
         }
        _context.SaveChanges();
    }
    public void UpdateGameFileName(int gameId, string fileName)
    {
        var game = _context.Games.Find(gameId);
        if (game != null)
        {
            game.SettingsFileName = fileName;
            _context.SaveChanges();
        }
    }

    public void CleanupTemporarySettings(string fileName)
    {
        var settingsToRemove = _context.GameSettings.Where(gs => gs.FileName == fileName);
        _context.GameSettings.RemoveRange(settingsToRemove);
        _context.SaveChanges();
    }
    
    public IEnumerable<string> GetDistinctFileNames()
    {
        return _context.GameSettings.Select(gs => gs.FileName).Distinct().ToList();
    }
    
    public string? PromptForPreSavedSettings()
    {
        var distinctFileNames = GetDistinctFileNames();

        Console.WriteLine("Available settings files:");
        foreach (var fName in distinctFileNames)
        {
            Console.WriteLine(fName);
        }

        Console.Write("Enter the name of the settings file you want to use: ");
        var input = Console.ReadLine()?.Trim();

        return input;
    }

}