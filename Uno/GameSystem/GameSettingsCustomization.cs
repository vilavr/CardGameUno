using DAL;
using Domain.Database;
using Newtonsoft.Json.Linq;

namespace GameSystem;

public class GameSettingsCustomization<T>
{
    public GameSettingsCustomization(string question, List<T> allowedValues, string key)
    {
        Question = question;
        AllowedValues = allowedValues;
        Key = key;
    }

    public string Question { get; }
    public List<T> AllowedValues { get; }
    public string Key { get; }

    public T GetUserInput()
    {
        T? userInput;
        while (true)
        {
            Console.WriteLine(Question);
            try
            {
                userInput = (T)Convert.ChangeType(Console.ReadLine()!, typeof(T));

                if (AllowedValues.Contains(userInput) || AllowedValues.Count == 0)
                    break; // exit the loop as we've got a valid input or it's validated somewhere else.
                Console.WriteLine(
                    $"Invalid selection. Please choose from the following: {string.Join(", ", AllowedValues)}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid format. Please try again.");
            }
            catch (InvalidCastException)
            {
                Console.WriteLine("This type of input is not supported.");
            }
        }

        return userInput;
    }

    public void UpdateSetting(T value, string settingsFileName, int? gameId, AppDbContext context)
    {
        // Begin transaction for atomic operation
        using var transaction = context.Database.BeginTransaction();

        try
        {
            // Console.WriteLine($"Checking existence of settings for file: {settingsFileName}");

            var settingsExist = context.GameSettings.Any(gs => gs.FileName == settingsFileName);

            if (!settingsExist)
            {
                // Console.WriteLine($"No settings found for {settingsFileName}, creating with default settings...");

                var defaultSettings = context.GameSettings
                    .Where(gs => gs.FileName == "Default")
                    .ToList();

                if (!defaultSettings.Any())
                {
                    // Console.WriteLine("No default settings found to copy.");
                }
                else
                {
                    foreach (var setting in defaultSettings)
                    {
                        var newSetting = new GameSetting
                        {
                            GameId = gameId,
                            FileName = settingsFileName,
                            SettingName = setting.SettingName,
                            SettingValue = setting.SettingValue
                        };
                        context.GameSettings.Add(newSetting);
                        // Console.WriteLine($"Adding setting {setting.SettingName} for file {settingsFileName}.");
                    }

                    int createdCount = context.SaveChanges();
                    // Console.WriteLine($"{createdCount} settings created for {settingsFileName}.");
                }
            }

            var settingToUpdate = context.GameSettings
                .FirstOrDefault(gs => gs.FileName == settingsFileName && gs.SettingName == Key);

            if (settingToUpdate != null)
            {
                // Console.WriteLine($"Found existing setting for {Key}, updating value...");
                settingToUpdate.SettingValue = value!.ToString();
            }
            else
            {
                // Console.WriteLine($"No existing setting found for {Key}, adding new setting...");
                var newSetting = new GameSetting
                {
                    GameId = gameId,
                    FileName = settingsFileName,
                    SettingName = Key,
                    SettingValue = value!.ToString()
                };
                context.GameSettings.Add(newSetting);
            }

            int changesSaved = context.SaveChanges();
            Console.WriteLine($"{changesSaved} changes saved to settings for {settingsFileName}.");

            // Commit transaction
            transaction.Commit();
        }
        catch (Exception ex)
        {
            // Rollback transaction on error
            transaction.Rollback();
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}