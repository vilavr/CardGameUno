using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MenuSystem;

public class CardQuantityPrompt
{
    private readonly JObject _settings;
    private readonly string _filePath;

    public CardQuantityPrompt(string filePath)
    {
        _filePath = filePath;
        string jsonData = File.ReadAllText(filePath);
        _settings = JObject.Parse(jsonData);
    }

    public void UpdateCardQuantity()
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
            JToken? cardSetting = _settings.SelectToken(jsonKey);

            if (cardSetting == null)
            {
                Console.WriteLine($"Card '{cardName.Replace("_", " ")}' not found.");
                return;
            }

            int currentQuantity = cardSetting.Value<int>();
            int newQuantity = currentQuantity + change;

            if (newQuantity < 0 || newQuantity > 4)
            {
                Console.WriteLine(
                    $"Invalid card quantity. It should still be between 0 and 4. Current quantity of '{cardName.Replace("_", " ")}' cards: {currentQuantity}");
                return;
            }

            cardSetting.Replace(newQuantity);

            File.WriteAllText(_filePath, _settings.ToString());

            Console.WriteLine($"Updated quantity of '{cardName.Replace("_", " ")}' cards to: {newQuantity}");
        }
        else
        {
            Console.WriteLine("Invalid command format.");
        }
    }
}