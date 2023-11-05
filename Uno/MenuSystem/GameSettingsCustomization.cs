using Newtonsoft.Json.Linq;

namespace MenuSystem;

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

    public void UpdateSetting(T value, GameSettingsCustomization<T> promptDetails)
    {
        // Absolute path to the Resources directory. This path might need to be updated according to your directory structure.
        var resourcesDirectory = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../")), "Resources");
        var settingsFileName =
            ApplicationState.SettingsFileName!; // Your current settings file name, retrieved from a global state.

        // Building the file path.
        var targetFilePath = Path.Combine(resourcesDirectory, settingsFileName);

        // Console.WriteLine($"Attempting to update settings file at: {targetFilePath}"); // Output target file path

        if (!File.Exists(targetFilePath))
        {
            var defaultFilePath = Path.Combine(resourcesDirectory, "default_settings.json");
            Console.WriteLine(
                $"File not found at {targetFilePath}. Attempting to create a new one from the default settings at {defaultFilePath}.");

            if (File.Exists(defaultFilePath))
            {
                File.Copy(defaultFilePath, targetFilePath);
                Console.WriteLine(
                    $"A new settings file has been created based on the default settings: {settingsFileName}");
            }
            else
            {
                Console.WriteLine($"Error: Default settings file not found: {defaultFilePath}");
                return;
            }
        }

        // Console.WriteLine($"Found settings file at: {targetFilePath}");
        // Read and parse the settings file.
        var jsonData = File.ReadAllText(targetFilePath);
        var jsonObject = JObject.Parse(jsonData);

        // Output for debugging: which key we're about to modify.
        // Console.WriteLine($"Preparing to update setting with key: {promptDetails.Key}");

        // Process the JSON to find the key and replace the value.
        var keyParts = promptDetails.Key.Split(':');
        JToken currentToken = jsonObject;
        for (var i = 0; i < keyParts.Length; i++)
        {
            currentToken = currentToken[keyParts[i]]!;

            if (currentToken == null)
                throw new KeyNotFoundException($"The key '{keyParts[i]}' was not found in the JSON structure.");

            if (i == keyParts.Length - 1)
            {
                var valueToReplace = JToken.FromObject(value); // Convert the new value to a JToken.
                currentToken.Replace(valueToReplace);
                // Console.WriteLine($"Value for '{keyParts[i]}' successfully replaced with: {value}");
            }
        }

        // Write the modified JSON back to the file.
        jsonData = jsonObject.ToString();
        File.WriteAllText(targetFilePath, jsonData);

        Console.WriteLine(
            $"Settings for '{promptDetails.Key}' have been successfully updated in file: {targetFilePath}");
    }
}