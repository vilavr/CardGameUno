using GameSystem;
using MenuSystem;
using DAL;
using DBoperations;
using Microsoft.EntityFrameworkCore;

SQLitePCL.Batteries.Init();

var contextOptions = new DbContextOptionsBuilder<AppDbContext>() 
    .UseSqlite("Data Source=/home/viralavrova/cardgameuno/Uno/ConsoleApp/myapp.db")
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
using (var context = new AppDbContext(contextOptions))
{
    // Call the method to insert default settings
    DefaultSettingsInitalization.InsertDefaultSettings(context);

    Console.WriteLine("Default settings have been successfully inserted into the database.");
    var cardQuantityChangePrompt = new CardSettingsCustomization(context);



    Menu mainMenu = null!;
    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string relativePathToProjectRoot =
        Path.Combine(Path.GetFullPath(Path.Combine(baseDirectory, "../../../../")), "Resources");


    var numberOfDecksPrompt = new GameSettingsCustomization<int>(
        "Enter the number of decks (1-4): ",
        Enumerable.Range(1, 4).ToList(),
        "NumberOfDecks"
    );

    var winningScorePrompt = new GameSettingsCustomization<int>(
        "Enter the winning score (100-1000): ",
        Enumerable.Range(1, 10).Select(i => i * 100).ToList(), // Will generate numbers 100, 200, ..., 1000.
        "WinningScore"
    );

    var directionPrompt = new GameSettingsCustomization<string>(
        "Enter playing direction (clockwise or counterclockwise): ",
        new List<string> { "clockwise", "counterclockwise" },
        "PlayDirection"
    );

    var numberOfCardsPrompt = new GameSettingsCustomization<int>(
        "Enter the number of cards per player (5-10): ",
        Enumerable.Range(5, 6).ToList(), // Allowing choices from 5 to 10 cards per player.
        "NumberOfCardsPerPlayer"
    );


    string PromptForSettingsFile()
    {
        Console.Write(
            "Enter the name of the settings file to save your customizations (Press Enter to change default settings): ");
        var filename = Console.ReadLine()!.Trim();
        if (string.IsNullOrWhiteSpace(filename))
        {
            filename = "Default"; 
            Console.WriteLine("No input provided. Using default settings.");
        }
        
        var fullPath = Path.Combine(relativePathToProjectRoot, filename);
    
        ApplicationState.SettingsFileName = filename;

        return filename;
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
                    int numberOfDecks = numberOfDecksPrompt.GetUserInput();
                    numberOfDecksPrompt.UpdateSetting(numberOfDecks, ApplicationState.SettingsFileName, null, context);
                    return "return";
                }
            },
            new()
            {
                Shortcut = "2",
                MenuLabel = "Winning score",
                MethodToRun = () =>
                {
                    int winningScore = winningScorePrompt.GetUserInput();
                    winningScorePrompt.UpdateSetting(winningScore, ApplicationState.SettingsFileName, null, context);

                    return "return";
                }
            },
            new()
            {
                Shortcut = "3",
                MenuLabel = "Playing direction",
                MethodToRun = () =>
                {
                    string playDirection = directionPrompt.GetUserInput();
                    directionPrompt.UpdateSetting(playDirection, ApplicationState.SettingsFileName, null, context);

                    return "return";
                }
            },
            new()
            {
                Shortcut = "4",
                MenuLabel = "Number of cards per player",
                MethodToRun = () =>
                {
                    int numberOfCards = numberOfCardsPrompt.GetUserInput();
                    numberOfCardsPrompt.UpdateSetting(numberOfCards, ApplicationState.SettingsFileName, null, context);

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
                // Using the GameSettingsService to handle the settings
                var settingsService = new GameSettingsService(context);
                int gameId = settingsService.CreateNewGame("Default");
                settingsService.EnsureSettingsForGame(gameId, "Default");
        
                // var gameEngine = new GameEngine(gameId, context);
                //
                // // Start the game with the default settings
                // gameEngine.StartGame();
                string settingsFilePath = Path.Combine(relativePathToProjectRoot, "settings_info.json");
                var gameEngine = new GameEngine(settingsFilePath);

                // Start the game
                gameEngine.StartGame();
                mainMenu.Run();

                return null;
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

                // Create a new game record
                var settingsService = new GameSettingsService(context);
                int gameId = settingsService.CreateNewGame(null); // Filename is null initially

                if (saveForFuture == "yes")
                {
                    Console.Write("Enter the name for the settings file (without extension): ");
                    var customSettingsFile = Console.ReadLine()?.Trim() ?? "custom_settings";
                    ApplicationState.SettingsFileName = customSettingsFile;
                    // Customization logic
                    kindofSettingsToCustomizeStartGame.Run();

                    // Update the game record with the new settings filename
                    settingsService.UpdateGameFileName(gameId, customSettingsFile);
                }
                else
                {
                    // Customization logic
                    ApplicationState.SettingsFileName = "settings_info";
                    kindofSettingsToCustomizeStartGame.Run();
                    settingsService = new GameSettingsService(context);
                    // Use a temporary file name to identify this game's settings
                    settingsService.UpdateGameFileName(gameId, "settings_info");
                }

                // Start the game with the customized settings
                // var gameEngine = new GameEngine(gameId, context);
                string settingsFilePath = Path.Combine(relativePathToProjectRoot, "settings_info.json");
                var gameEngine = new GameEngine(settingsFilePath);
                gameEngine.StartGame();
                mainMenu.Run();

                if (saveForFuture != "yes")
                {
                    // Cleanup settings for "settings_info" if they were not meant to be saved
                    settingsService.CleanupTemporarySettings("settings_info");
                }

                return null;
            }
        },
        new()
        {
            Shortcut = "3",
            MenuLabel = "Use pre-saved settings",
            MethodToRun = () =>
            {
                var settingsService = new GameSettingsService(context);
                // Prompt user for the filename of pre-saved settings
                var preSavedSettingsFileName = settingsService.PromptForPreSavedSettings();

                // Create a new game record and apply pre-saved settings
                settingsService = new GameSettingsService(context);
                int gameId = settingsService.CreateNewGame(preSavedSettingsFileName);
                settingsService.ApplyPreSavedSettings(preSavedSettingsFileName, gameId);

                // Start the game with the pre-saved settings
                // var gameEngine = new GameEngine(gameId, context);
                string settingsFilePath = Path.Combine(relativePathToProjectRoot, "settings_info.json");
                var gameEngine = new GameEngine(settingsFilePath);
                gameEngine.StartGame();
                mainMenu.Run();

                return null;
            }
        }
    }, EMenuLevel.Second);
    mainMenu = new Menu("Main menu", new List<MenuItem>
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
            MenuLabel = "Load game",
            MethodToRun = () =>
            {
                var gameSaver = new GameSaver();

                // Prompting the user to select a save file and obtaining the file path.
                if (gameSaver.PromptUserForLoad(out string filePath))
                {
                    var gameState = gameSaver.LoadGame(filePath);

                    if (gameState != null)
                    {
                        string settingsFilePath = Path.Combine(relativePathToProjectRoot, "settings_info.json");
                        var gameEngine = new GameEngine(settingsFilePath);

                        // Start the game
                        gameEngine.StartGame(didUserSaveGame: true, savedGameFilePath: filePath);
                    }
                    else
                    {
                        Console.WriteLine("Error loading game. Please start a new game or try loading again.");
                    }
                }
                else
                {
                    Console.WriteLine("Load game cancelled or no save files available. Returning to main menu.");
                }

                return null;
            }
        }
    });


    while (true)
    {
        var result = mainMenu.Run();

        if (result == "x")
            break;
        if (result == "m") continue;
    }
}