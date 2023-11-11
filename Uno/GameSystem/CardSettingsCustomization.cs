using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DAL;
using Domain.Database;  // Replace with your actual namespace for domain entities
using Microsoft.EntityFrameworkCore;

namespace GameSystem;

public class CardSettingsCustomization
{
    private readonly AppDbContext _context;
    private readonly string _defaultFileName = "Default"; 

    public CardSettingsCustomization(AppDbContext context)
    {
        _context = context;
    }
    private (string cardName, int newQuantity, int currentQuantity)? ValidateAndNormalizeInput(string input, string fileName)
    {
        input = input.Trim();
    
        var match = Regex.Match(input, @"(?<card>[a-zA-Z]+(?:\s+[a-zA-Z]+)*\s*\d*)\s+(?<operation>[+-]\d+)");
        if (!match.Success)
        {
            return null;
        }
    
        // Normalize card name
        var cardName = GetNormalizedCardName(match.Groups["card"].Value.Trim());

        if (string.IsNullOrEmpty(cardName))
        {
            return null;
        }

        // Validate and calculate the new card quantity
        if (!int.TryParse(match.Groups["operation"].Value, out int change))
        {
            return null;
        }
    
        // Retrieve the setting from the database
        var setting = _context.GameSettings
            .Where(gs => gs.FileName == fileName && gs.SettingName == cardName)
            .Select(gs => new { gs.SettingValue })
            .SingleOrDefault();

        if (setting == null)
        {
            return null;
        }

        if (!int.TryParse(setting.SettingValue, out int currentQuantity))
        {
            return null;
        }

        var newQuantity = currentQuantity + change;

        // Check if new quantity falls within the acceptable range
        if (newQuantity < 0 || newQuantity > 4)
        {
            Console.WriteLine($"Invalid quantity. The number of '{cardName.Replace("_", " ")}' cards must be between 0 and 4.");
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
        var targetFileName = string.IsNullOrEmpty(settingsFileName) ? _defaultFileName : settingsFileName;

        // Check if settings for the given FileName already exist
        var settingsExist = _context.GameSettings.Any(gs => gs.FileName == targetFileName);

        if (!settingsExist)
        {
            // If settings do not exist, create a copy of all default entries with the new FileName
            var defaultSettings = _context.GameSettings
                .Where(gs => gs.FileName == _defaultFileName)
                .ToList();

            foreach (var setting in defaultSettings)
            {
                _context.GameSettings.Add(new GameSetting
                {
                    FileName = targetFileName,
                    SettingName = setting.SettingName,
                    SettingValue = setting.SettingValue
                });
            }

            _context.SaveChanges();
            Console.WriteLine($"A new settings file has been created based on the default settings: {settingsFileName}");
        }

        // Loop for multiple updates
        while (true)
        {
            Console.WriteLine("\nEnter the card update command (e.g., 'Red 0 +2') or press 'f' to finish:");
            var input = Console.ReadLine();

            if (string.Equals(input?.Trim(), "f", StringComparison.OrdinalIgnoreCase)) break; // Exit the loop if the user wants to finish

            // Inside your loop in the UpdateCardQuantity method
            var validationResult = ValidateAndNormalizeInput(input, targetFileName);
            if (validationResult == null)
            {
                Console.WriteLine("Invalid command or card name, or the quantity was out of range. Please try again.");
                continue;
            }


            var (cardName, newQuantity, _) = validationResult.Value;

            // Retrieve the setting from the database
            var settingToUpdate = _context.GameSettings
                .SingleOrDefault(gs => gs.FileName == targetFileName && gs.SettingName == cardName);

            if (settingToUpdate != null)
            {
                // Update the existing setting
                settingToUpdate.SettingValue = newQuantity.ToString();
                _context.SaveChanges();
                Console.WriteLine($"Updated quantity of '{cardName.Replace("_", " ")}' cards to: {newQuantity} in file: {targetFileName}");
            }
            else
            {
                Console.WriteLine($"Error: Setting '{cardName}' not found for file '{targetFileName}'.");
            }
        }
    }
}