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

    public void UpdateSetting(T value, string settingsFileName, AppDbContext context)
    {
        // Check if any settings exist for the given FileName
        var settingsExist = context.GameSettings.Any(gs => gs.FileName == settingsFileName);

        if (!settingsExist)
        {
            // If settings do not exist, create a copy of all default entries with the new FileName
            var defaultSettings = context.GameSettings
                .Where(gs => gs.FileName == "Default")
                .ToList();

            foreach (var Setting in defaultSettings)
            {
                context.GameSettings.Add(new GameSetting
                {
                    FileName = settingsFileName,
                    SettingName = Setting.SettingName,
                    SettingValue = Setting.SettingValue
                });
            }

            context.SaveChanges();
            Console.WriteLine($"A new settings file has been created based on the default settings: {settingsFileName}");
        }

        // Retrieve the specific setting to update or create a new one if it doesn't exist
        var setting = context.GameSettings
            .SingleOrDefault(gs => gs.FileName == settingsFileName && gs.SettingName == Key);

        if (setting != null)
        {
            // Update the existing setting
            setting.SettingValue = value!.ToString();
        }
        else
        {
            // Add a new setting if it does not exist
            context.GameSettings.Add(new GameSetting
            {
                FileName = settingsFileName,
                SettingName = Key,
                SettingValue = value!.ToString()
            });
        }
        context.SaveChanges();
        Console.WriteLine($"Setting '{Key}' updated to: {value} for file: {settingsFileName}");
    }
}