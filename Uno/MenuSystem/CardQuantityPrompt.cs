using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MenuSystem
{
    public class CardQuantityPrompt
    {
        private JObject _settings;
        private string _filePath;

        public CardQuantityPrompt(string filePath)
        {
            _filePath = filePath;
            string jsonData = File.ReadAllText(filePath);
            _settings = JObject.Parse(jsonData);
        }

        private (string cardName, int newQuantity, int currentQuantity)? ValidateAndNormalizeInput(string input)
        {
            input = input.Trim();
            // Console.WriteLine($"Debug: Input command: '{input}'");

            var match = Regex.Match(input, @"(?<card>[a-zA-Z]+(?:\s+[a-zA-Z]+)*\s*\d*)\s+(?<operation>[+-]\d+)");
            if (!match.Success)
            {
                // Console.WriteLine("Debug: Regex match was not successful.");
                return null;
            }

            // Normalize card name
            string cardName = GetNormalizedCardName(match.Groups["card"].Value.Trim());
            // Console.WriteLine($"Debug: Normalized card name: '{cardName}'");

            if (string.IsNullOrEmpty(cardName))
            {
                // Console.WriteLine("Debug: Card name after normalization is empty.");
                return null;
            }

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

            string jsonKey = "cardSettings." + cardName;
            JToken? cardSetting = _settings.SelectToken(jsonKey);

            if (cardSetting == null)
            {
                // Console.WriteLine($"Debug: No settings found for card '{cardName}'.");
                return null;
            }

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

            int newQuantity = currentQuantity + change;
            // Console.WriteLine($"Debug: New quantity: {newQuantity}");

            // Check if new quantity falls within the acceptable range
            if (newQuantity < 0 || newQuantity > 4)
            {
                // Console.WriteLine("Debug: New quantity is out of the allowed range.");
                return null;
            }

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

            string cardName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cardNameRaw.ToLower());
            cardName = Regex.Replace(cardName, @"\s+", "_");

            if (wildDrawFourRepresentations.Contains(cardNameRaw))
            {
                return "Wild_DrawFour";
            }

            bool isDrawTwoCard = drawTwoRepresentations.Any(representation =>
                cardNameRaw.EndsWith(representation, StringComparison.OrdinalIgnoreCase)
            );

            if (isDrawTwoCard)
            {
                string colorPart = Regex.Match(cardNameRaw, @"^[a-zA-Z]+").Value; // Capture the color part only.
                colorPart = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(colorPart.ToLower());
                return $"{colorPart}_DrawTwo";
            }

            return cardName;
        }

         public void UpdateCardQuantity(string inputFileName = "default_settings.json")
    {
        // Absolute path to the Resources directory
        string resourcesDirectory = "/home/viralavrova/cardgameuno/Uno/Resources";

        // Asking for a custom file name from the user
        Console.WriteLine("If you want to use a custom settings file, please enter the file name (otherwise, press Enter to continue):");
        string userInputFileName = Console.ReadLine()?.Trim() ?? string.Empty;

        // Determine whether to use the default file name or the user-provided one
        string targetFileName = string.IsNullOrEmpty(userInputFileName) ? inputFileName : userInputFileName;

        // Check if the file name ends with '.json', if not, append it
        if (!targetFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            targetFileName += ".json";
        }

        // Construct the full path to the target file
        string targetFilePath = Path.Combine(resourcesDirectory, targetFileName);

        // If the target file doesn't exist, create it and copy the contents from the default file
        if (!File.Exists(targetFilePath))
        {
            // Use the default settings file as a template
            string defaultFilePath = Path.Combine(resourcesDirectory, "default_settings.json");
            if (File.Exists(defaultFilePath))
            {
                File.Copy(defaultFilePath, targetFilePath);
                Console.WriteLine($"A new settings file has been created based on the default settings: {targetFileName}");
            }
            else
            {
                Console.WriteLine($"Error: Default settings file not found: {defaultFilePath}");
                return;
            }
        }

        // Load the settings from the target file
        _filePath = targetFilePath;
        string jsonData = File.ReadAllText(_filePath);
        _settings = JObject.Parse(jsonData);
        
        Console.WriteLine("Enter the card update command (e.g., 'Red 0 +2'):");
        string? input = Console.ReadLine();
        
        var validationResult = ValidateAndNormalizeInput(input);
        if (validationResult == null)
        {
            Console.WriteLine("Invalid command or card name, or the quantity was out of range.");
            return;
        }

        var (cardName, newQuantity, currentQuantity) = validationResult.Value;

        if (newQuantity == currentQuantity)
        {
            Console.WriteLine($"Quantity of '{cardName.Replace("_", " ")}' cards is already {currentQuantity}. No changes were made.");
            return;
        }

        // Update the quantity in the settings
        string jsonKey = "cardSettings." + cardName;
        _settings[jsonKey] = newQuantity;

        // Save the updated settings back to the file
        File.WriteAllText(_filePath, _settings.ToString());

        Console.WriteLine($"Updated quantity of '{cardName.Replace("_", " ")}' cards in '{targetFileName}' to: {newQuantity}");
    }
    }
}
