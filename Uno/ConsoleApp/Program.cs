using MenuSystem;

var numberOfDecksPrompt = new GameSettingsCustomization<int>(
    "Enter the number of decks (1-4): ",
    Enumerable.Range(1, 4).ToList(), // Allowing 1 to 4 decks for variety.
    "gameSettings:NumberOfDecks"
);

var winningScorePrompt = new GameSettingsCustomization<int>(
    "Enter the winning score (100-1000): ",
    Enumerable.Range(1, 10).Select(i => i * 100).ToList(), // Will generate numbers 100, 200, ..., 1000.
    "gameSettings:WinningScore"
);

var directionPrompt = new GameSettingsCustomization<string>(
    "Enter playing direction (clockwise or counterclockwise): ",
    new List<string> { "clockwise", "counterclockwise" },
    "gameSettings:PlayDirection"
);


var cardQuantityChangePrompt =
    new CardSettingsCustomization("/home/viralavrova/cardgameuno/Uno/Resources/default_settings.json");

string PromptForSettingsFile()
{
    Console.Write(
        "Enter the name of the settings file to save your customizations (Press Enter to change default settings): ");
    var filename = Console.ReadLine()!.Trim();
    if (string.IsNullOrWhiteSpace(filename))
    {
        filename = "default_settings.json";
        Console.WriteLine("No input provided. Using default settings.");
    }
    else if (!filename.EndsWith(".json"))
    {
        filename += ".json";
    }

    var fullPath = Path.Combine("/home/viralavrova/cardgameuno/Uno/Resources", filename);
    ApplicationState.SettingsFileName = fullPath;

    return fullPath;
}


var cardSettingsToCustomize = new Menu("Choose which card setting you'd like to customize", new List<MenuItem>
{
    new()
    {
        Shortcut = "1",
        MenuLabel = "Number of any card in the deck",
        MethodToRun = () =>
        {
            try
            {
                var currentSettingsFile = ApplicationState.SettingsFileName!;
                cardQuantityChangePrompt.UpdateCardQuantity(currentSettingsFile);
            }
            catch (Exception
                   ex) // It's good practice to handle potential exceptions, especially when dealing with file operations or user input.
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return "return"; // Return to the previous menu after the operation
        }
    }
}, EMenuLevel.Other);

var gameSettingsToCustomize = new Menu(
    "Choose which game setting you'd like to customize",
    new List<MenuItem>
    {
        new()
        {
            Shortcut = "1",
            MenuLabel = "Number of decks",
            MethodToRun = () =>
            {
                var userChoice = numberOfDecksPrompt.GetUserInput();
                // UpdateSetting now gets the key from the promptDetails and doesn't need a path.
                numberOfDecksPrompt.UpdateSetting(userChoice, numberOfDecksPrompt);

                Console.WriteLine($"Number of decks updated to: {userChoice}");
                return "return";
            }
        },
        new()
        {
            Shortcut = "2",
            MenuLabel = "Winning score",
            MethodToRun = () =>
            {
                var userChoice = winningScorePrompt.GetUserInput();
                winningScorePrompt.UpdateSetting(userChoice, winningScorePrompt);

                Console.WriteLine($"Winning score updated to: {userChoice}");
                return "return";
            }
        },
        new()
        {
            Shortcut = "3",
            MenuLabel = "Playing direction",
            MethodToRun = () =>
            {
                var userChoice = directionPrompt.GetUserInput();
                directionPrompt.UpdateSetting(userChoice, directionPrompt);

                Console.WriteLine($"Playing direction updated to: {userChoice}");
                return "return";
            }
        }
    },
    EMenuLevel.Other
);

var kindofSettingsToCustomizeSettings = new Menu("Choose what category of settings you'd like to customize",
    new List<MenuItem>
    {
        new()
        {
            Shortcut = "1",
            MenuLabel = "Card settings",
            MethodToRun = cardSettingsToCustomize.Run
        },
        new()
        {
            Shortcut = "2",
            MenuLabel = "Game settings",
            MethodToRun = gameSettingsToCustomize.Run
        }
    }, EMenuLevel.Second);

var kindofSettingsToCustomizeStartGame = new Menu("Choose what category of settings you'd like to customize",
    new List<MenuItem>
    {
        new()
        {
            Shortcut = "1",
            MenuLabel = "Card settings",
            MethodToRun = cardSettingsToCustomize.Run
        },
        new()
        {
            Shortcut = "2",
            MenuLabel = "Game settings",
            MethodToRun = gameSettingsToCustomize.Run
        },
        new()
        {
            Shortcut = "3",
            MenuLabel = "Finish customization",
            MethodToRun = () => { return "back"; }
        }
    }, EMenuLevel.Other);

var settingsChoice = new Menu("Choose what settings you want to continue with", new List<MenuItem>
{
    new()
    {
        Shortcut = "1",
        MenuLabel = "Default settings",
        MethodToRun = () =>
        {
            // Managing settings files.
            var settingsManager = new SettingsFileManager("/home/viralavrova/cardgameuno/Uno/Resources");
            settingsManager.PromptAndCopySettings(true);

            // Setting up the game with players and deck
            var gameSetup = new GameSetup();
            var players = gameSetup.CreatePlayers();
            var deck = new CardDeck("/home/viralavrova/cardgameuno/Uno/Resources/settings_info.json"); 
            deck.InitializeDeck(); 
            return players;
        }

    },
    new()
    {
        Shortcut = "2",
        MenuLabel = "Customise settings",
        MethodToRun = () =>
        {
            Console.Write("Do you want to save these customizations for future use? (yes/no): ");
            var saveForFuture = Console.ReadLine()?.Trim().ToLower() ?? "no";

            string customSettingsFilePath;
            var settingsManager = new SettingsFileManager("/home/viralavrova/cardgameuno/Uno/Resources");
            var targetSettingsFilePath =
                Path.Combine("/home/viralavrova/cardgameuno/Uno/Resources", "settings_info.json");

            if (saveForFuture == "yes")
            {
                Console.Write("Enter the name for the settings file (without extension): ");
                var customSettingsFile = Console.ReadLine()?.Trim() ?? "custom_settings";
                customSettingsFilePath = Path.Combine("/home/viralavrova/cardgameuno/Uno/Resources",
                    customSettingsFile + ".json");

                if (!File.Exists(customSettingsFilePath))
                    settingsManager.CopyContentsToSettingsInfo(
                        Path.Combine("/home/viralavrova/cardgameuno/Uno/Resources", "default_settings.json"),
                        customSettingsFilePath);

                // Set the custom settings file as the one to use for this session.
                ApplicationState.SettingsFileName = customSettingsFilePath;
                kindofSettingsToCustomizeStartGame.Run();
                settingsManager.CopyContentsToSettingsInfo(customSettingsFilePath, targetSettingsFilePath);
            }
            else
            {
                var defaultSettingsFilePath = "/home/viralavrova/cardgameuno/Uno/Resources/default_settings.json";
                settingsManager.CopyContentsToSettingsInfo(defaultSettingsFilePath, targetSettingsFilePath);

                // Set 'settings_info.json' as the file to use for this session.
                ApplicationState.SettingsFileName = targetSettingsFilePath;
                kindofSettingsToCustomizeStartGame.Run();
            }

            var gameSetup = new GameSetup();
            var players = gameSetup.CreatePlayers();
            var deck = new CardDeck("/home/viralavrova/cardgameuno/Uno/Resources/settings_info.json"); 
            deck.InitializeDeck(); 
            return players;
        }
    },
    new()
    {
        Shortcut = "3",
        MenuLabel = "Use pre-saved settings",
        MethodToRun = () =>
        {
            var settingsManager = new SettingsFileManager("/home/viralavrova/cardgameuno/Uno/Resources");
            settingsManager.PromptAndCopySettings(false);

            var gameSetup = new GameSetup();
            return gameSetup.CreatePlayers();
        }
    }
}, EMenuLevel.Second);
var mainMenu = new Menu("Main menu", new List<MenuItem>
{
    new()
    {
        Shortcut = "1",
        MenuLabel = "Start game",
        MethodToRun = settingsChoice.Run
    },
    new()
    {
        Shortcut = "2",
        MenuLabel = "Settings",
        MethodToRun = () =>
        {
            PromptForSettingsFile();
            kindofSettingsToCustomizeSettings.Run();
            return null;
        }
    },
    new()
    {
        Shortcut = "3",
        MenuLabel = "Load game"
    }
});

while (true)
{
    var result = mainMenu.Run();

    if (result == "x")
        break;
    if (result == "m") continue;
}