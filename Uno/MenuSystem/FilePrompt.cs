using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MenuSystem
{
    public class FilePrompt
    {
        private readonly JObject _settings;
        private readonly string _defaultFilePath;

        public FilePrompt(string defaultFilePath)
        {
            _defaultFilePath = defaultFilePath;
            string jsonData = File.ReadAllText(defaultFilePath);
            _settings = JObject.Parse(jsonData);
        }

        public (string key, int change)? ValidateAndNormalizeCardInput(string inputCommand)
        {
            Console.WriteLine("Enter the card update command (e.g., 'Red 0 +2'):");
            string input = Console.ReadLine()!;

            input = input.Trim();

            var match = Regex.Match(input, @"(?<card>[a-zA-Z]+(?:\s+[a-zA-Z]+)*\s*\d*)\s+(?<operation>[+-]\d+)");
            if (match.Success)
            {
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

                string cardNameRaw = match.Groups["card"].Value.Trim();
                string cardName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cardNameRaw.ToLower());
                cardName = Regex.Replace(cardName, @"\s+", "_");

                if (wildDrawFourRepresentations.Contains(cardNameRaw))
                {
                    cardName = "Wild_DrawFour";
                }

                bool isDrawTwoCard = drawTwoRepresentations.Any(representation =>
                    cardNameRaw.EndsWith(representation, StringComparison.OrdinalIgnoreCase)
                );

                if (isDrawTwoCard)
                {
                    string colorPart = Regex.Match(cardNameRaw, @"^[a-zA-Z]+").Value; // Capture the color part only.
                    colorPart = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(colorPart.ToLower());
                    cardName = $"{colorPart}_DrawTwo";
                }

                int change = int.Parse(match.Groups["operation"].Value);
                string jsonKey = "cardSettings." + cardName;

                return (jsonKey, change);
            }

            return null;
        }

        public void UpdateSettingsInFile(string fileName, string key, int change)
        {
            string targetFilePath = fileName.Equals("default_settings.json", StringComparison.OrdinalIgnoreCase)
                ? _defaultFilePath
                : Path.Combine(Path.GetDirectoryName(_defaultFilePath), fileName);

            JObject targetSettings;

            if (File.Exists(targetFilePath))
            {
                string jsonData = File.ReadAllText(targetFilePath);
                targetSettings = JObject.Parse(jsonData);
            }
            else
            {
                targetSettings = (JObject)_settings.DeepClone();
            }

            JToken? cardSetting = targetSettings.SelectToken(key);

            if (cardSetting == null)
            {
                Console.WriteLine($"Card '{key.Replace("cardSettings.", "").Replace("_", " ")}' not found.");
                return;
            }

            int currentQuantity = cardSetting.Value<int>();
            int newQuantity = currentQuantity + change;

            if (newQuantity < 0 || newQuantity > 4)
            {
                Console.WriteLine($"Invalid card quantity. It should be between 0 and 4. Current quantity of '{key.Replace("cardSettings.", "").Replace("_", " ")}' cards: {currentQuantity}");
                return;
            }

            cardSetting.Replace(newQuantity);

            File.WriteAllText(targetFilePath, targetSettings.ToString());

            Console.WriteLine($"Updated quantity of '{key.Replace("cardSettings.", "").Replace("_", " ")}' cards to: {newQuantity}");
        }
    }
}
