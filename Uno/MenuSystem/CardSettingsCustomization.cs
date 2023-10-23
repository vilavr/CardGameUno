using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MenuSystem;

public class CardSettingsCustomization
{
    private string _filePath;
    private JObject _settings;

    public CardSettingsCustomization(string filePath)
    {
        _filePath = filePath;
        var jsonData = File.ReadAllText(filePath);
        _settings = JObject.Parse(jsonData);
    }

    private (string cardName, int newQuantity, int currentQuantity)? ValidateAndNormalizeInput(string input)
    {
        input = input.Trim();
        // Console.WriteLine($"Debug: Input command: '{input}'");

        var match = Regex.Match(input, @"(?<card>[a-zA-Z]+(?:\s+[a-zA-Z]+)*\s*\d*)\s+(?<operation>[+-]\d+)");
        if (!match.Success)
            // Console.WriteLine("Debug: Regex match was not successful.");
            return null;

        // Normalize card name
        var cardName = GetNormalizedCardName(match.Groups["card"].Value.Trim());
        // Console.WriteLine($"Debug: Normalized card name: '{cardName}'");

        if (string.IsNullOrEmpty(cardName))
            // Console.WriteLine("Debug: Card name after normalization is empty.");
            return null;

        // Validate and calculate the new card quantity
        int change;
        try
        {
            change = int.Parse(match.Groups["operation"].Value);
            // Console.WriteLine($"Debug: Change value: {change}");
        }
        catch (FormatException)
        {
            // Console.WriteLine("Debug: Operation value is not a valid integer.");
            return null;
        }

        var jsonKey = "cardSettings." + cardName;
        var cardSetting = _settings.SelectToken(jsonKey);

        if (cardSetting == null)
            // Console.WriteLine($"Debug: No settings found for card '{cardName}'.");
            return null;

        int currentQuantity;
        try
        {
            currentQuantity = cardSetting.Value<int>();
            // Console.WriteLine($"Debug: Current quantity: {currentQuantity}");
        }
        catch (Exception)
        {
            // Console.WriteLine("Debug: Current quantity is not an integer.");
            return null;
        }

        var newQuantity = currentQuantity + change;
        // Console.WriteLine($"Debug: New quantity: {newQuantity}");

        // Check if new quantity falls within the acceptable range
        if (newQuantity < 0 || newQuantity > 4)
            // Console.WriteLine("Debug: New quantity is out of the allowed range.");
            return null;

        return (cardName, newQuantity, currentQuantity);
    }

    private string GetNormalizedCardName(string cardNameRaw)
    {
        // Define representations
        var wildDrawFourRepresentations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "wild draw four",
            "wild drawfour",
            "Wild DrawFour",
            "wild draw 4",
            "Wild Draw Four",
            "Wild_Draw_Four"
        };

        var drawTwoRepresentations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "draw two",
            "drawtwo",
            "DrawTwo",
            "draw 2",
            "Draw 2"
        };

        var cardName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cardNameRaw.ToLower());
        cardName = Regex.Replace(cardName, @"\s+", "_");

        if (wildDrawFourRepresentations.Contains(cardNameRaw)) return "Wild_DrawFour";

        var isDrawTwoCard = drawTwoRepresentations.Any(representation =>
            cardNameRaw.EndsWith(representation, StringComparison.OrdinalIgnoreCase)
        );

        if (isDrawTwoCard)
        {
            var colorPart = Regex.Match(cardNameRaw, @"^[a-zA-Z]+").Value; // Capture the color part only.
            colorPart = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(colorPart.ToLower());
            return $"{colorPart}_DrawTwo";
        }

        return cardName;
    }

    public void UpdateCardQuantity(string settingsFileName)
    {
        // Absolute path to the Resources directory
        var resourcesDirectory = "/home/viralavrova/cardgameuno/Uno/Resources";

        // Complete file path
        var targetFilePath = Path.Combine(resourcesDirectory, settingsFileName);

        // If the target file doesn't exist, create it and copy the contents from the default file
        if (!File.Exists(targetFilePath))
        {
            // Use the default settings file as a template
            var defaultFilePath = Path.Combine(resourcesDirectory, "default_settings.json");
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

        // Load the settings from the target file
        _filePath = targetFilePath;
        var jsonData = File.ReadAllText(_filePath);
        _settings = JObject.Parse(jsonData);

        // Loop for multiple updates
        while (true)
        {
            Console.WriteLine("\nEnter the card update command (e.g., 'Red 0 +2') or press 'f' to finish:");
            var input = Console.ReadLine();

            if (input?.Trim().ToLower() == "f") break; // Exit the loop if the user wants to finish

            var validationResult = ValidateAndNormalizeInput(input);
            if (validationResult == null)
            {
                Console.WriteLine("Invalid command or card name, or the quantity was out of range. Please try again.");
                continue;
            }

            var (cardName, newQuantity, currentQuantity) = validationResult.Value;

            // Inside your loop where you update each card, replace the updating section with this:

            if (newQuantity == currentQuantity)
            {
                Console.WriteLine(
                    $"Quantity of '{cardName.Replace("_", " ")}' cards is already {currentQuantity}. No changes were made.");
            }
            else
            {
                // Navigate to the nested card settings and update the value there.
                var cardSettings = (JObject)_settings["cardSettings"]!; // Access the 'cardSettings' object.
                cardSettings[cardName] = newQuantity; // Directly update the value inside the nested structure.
                Console.WriteLine($"Updated quantity of '{cardName.Replace("_", " ")}' cards to: {newQuantity}");
            }
        }

        // After all changes, save the updated settings back to the file
        File.WriteAllText(_filePath, _settings.ToString());
        Console.WriteLine("\nAll changes saved successfully.");
    }
}