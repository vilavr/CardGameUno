using DAL;
using Domain.Database;

namespace DBoperations;

public class DefaultSettingsInitalization
{
    public static void InsertDefaultSettings(AppDbContext context)
    {
        // List of default settings
        var settings = new List<GameSetting>
        {
            // Card settings
            new GameSetting { SettingName = "Red_0", SettingValue = "2"},
            new GameSetting { SettingName = "Red_1", SettingValue = "2" },
            new GameSetting { SettingName = "Red_2", SettingValue = "2" },
            new GameSetting { SettingName = "Red_3", SettingValue = "2" },
            new GameSetting { SettingName = "Red_4", SettingValue = "2" },
            new GameSetting { SettingName = "Red_5", SettingValue = "2" },
            new GameSetting { SettingName = "Red_6", SettingValue = "2" },
            new GameSetting { SettingName = "Red_7", SettingValue = "2" },
            new GameSetting { SettingName = "Red_8", SettingValue = "2" },
            new GameSetting { SettingName = "Red_9", SettingValue = "2" },
            new GameSetting { SettingName = "Red_DrawTwo", SettingValue = "2" },
            new GameSetting { SettingName = "Red_Skip", SettingValue = "2" },
            new GameSetting { SettingName = "Red_Reverse", SettingValue = "2" },
            
            new GameSetting { SettingName = "Yellow_0", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_1", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_2", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_3", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_4", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_5", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_6", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_7", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_8", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_9", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_DrawTwo", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_Skip", SettingValue = "2" },
            new GameSetting { SettingName = "Yellow_Reverse", SettingValue = "2" },
            
            new GameSetting { SettingName = "Green_0", SettingValue = "2" },
            new GameSetting { SettingName = "Green_1", SettingValue = "2" },
            new GameSetting { SettingName = "Green_2", SettingValue = "2" },
            new GameSetting { SettingName = "Green_3", SettingValue = "2" },
            new GameSetting { SettingName = "Green_4", SettingValue = "2" },
            new GameSetting { SettingName = "Green_5", SettingValue = "2" },
            new GameSetting { SettingName = "Green_6", SettingValue = "2" },
            new GameSetting { SettingName = "Green_7", SettingValue = "2" },
            new GameSetting { SettingName = "Green_8", SettingValue = "2" },
            new GameSetting { SettingName = "Green_9", SettingValue = "2" },
            new GameSetting { SettingName = "Green_DrawTwo", SettingValue = "2" },
            new GameSetting { SettingName = "Green_Skip", SettingValue = "2" },
            new GameSetting { SettingName = "Green_Reverse", SettingValue = "2" },
            
            new GameSetting { SettingName = "Blue_0", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_1", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_2", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_3", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_4", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_5", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_6", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_7", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_8", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_9", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_DrawTwo", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_Skip", SettingValue = "2" },
            new GameSetting { SettingName = "Blue_Reverse", SettingValue = "2" },
            
            new GameSetting { SettingName = "Wild", SettingValue = "4" },
            new GameSetting { SettingName = "Wild_DrawFour", SettingValue = "4" },
            // Game settings
            new GameSetting { SettingName = "NumberOfDecks", SettingValue = "1" },
            new GameSetting { SettingName = "NumberOfCardsPerPlayer", SettingValue = "7" },
            new GameSetting { SettingName = "WinningScore", SettingValue = "500" },
            new GameSetting { SettingName = "PlayDirection", SettingValue = "counterclockwise" }
        };

        // Insert settings into the database
        foreach (var setting in settings)
        {
            // Add only if the setting does not already exist
            if (!context.GameSettings.Any(gs => gs.SettingName == setting.SettingName))
            {
                context.GameSettings.Add(setting);
            }
        }

        context.SaveChanges();
    }
}

