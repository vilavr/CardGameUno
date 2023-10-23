using MenuSystem;

var numberOfDecksPrompt = new GeneralPrompt<int>(
    "Enter the number of decks (1-4): ", 
    Enumerable.Range(1, 4).ToList() // Allowing 1 to 4 decks for variety.
);

var winningScorePrompt = new GeneralPrompt<int>(
    "Enter the winning score (100-1000): ", 
    Enumerable.Range(100, 10).Select(i => i * 10).ToList() // Allows increments of 100, from 100 to 1000.
);

var directionPrompt = new GeneralPrompt<string>(
    "Enter playing direction (clockwise or counterclockwise): ", 
    new List<string> { "clockwise", "counterclockwise" }
);

var cardQuantityChangePrompt = new CardQuantityPrompt("/home/viralavrova/cardgameuno/Uno/Resources/default_settings.json");

var cardSettingsToCustomize = new Menu(title:"Choose which card setting you'd like to customize", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Number of any card in the deck",
        MethodToRun = () => 
        {
            try
            {
                cardQuantityChangePrompt.UpdateCardQuantity();
            }
            catch (Exception ex) // It's good practice to handle potential exceptions, especially when dealing with file operations or user input.
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return "back";  // Return to the previous menu after the operation
        }
    },
}, EMenuLevel.Other);

var gameSettingsToCustomize = new Menu(title:"Choose which game setting you'd like to customize", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Number of decks",
        MethodToRun = () => 
        {
            int userChoice = numberOfDecksPrompt.GetUserInput();
            numberOfDecksPrompt.UpdateSetting("gameSettings:NumberOfDecks", userChoice, "/home/viralavrova/cardgameuno/Uno/Resources/default_settings.json");

            Console.WriteLine($"Number of decks updated to: {userChoice}");
            return "back";
        }
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Winning score",
        MethodToRun = () => 
        {
            int userChoice = winningScorePrompt.GetUserInput();
            winningScorePrompt.UpdateSetting("gameSettings:WinningScore", userChoice, "/home/viralavrova/cardgameuno/Uno/Resources/default_settings.json");

            Console.WriteLine($"Winning score updated to: {userChoice}");
            return "back";
        }
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Playing direction",
        MethodToRun = () => 
        {
            string userChoice = directionPrompt.GetUserInput();
            directionPrompt.UpdateSetting("gameSettings:PlayDirection", userChoice, "/home/viralavrova/cardgameuno/Uno/Resources/default_settings.json");

            Console.WriteLine($"Playing direction updated to: {userChoice}");
            return "back";  
        }    }
}, EMenuLevel.Other);

var kindofSettingsToCustomize = new Menu(title:"Choose what category of settings you'd like to customize", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Card settings",
        MethodToRun = cardSettingsToCustomize.Run
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Game settings",
        MethodToRun = gameSettingsToCustomize.Run
    }
}, EMenuLevel.Other);

var settingsChoice = new Menu(title:"Choose what settings you want to continue with", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Default settings",
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Customise settings",
        MethodToRun = kindofSettingsToCustomize.Run,
    },
    
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Use pre-saved settings"
    }
}, EMenuLevel.Second);
var mainMenu = new Menu(title:"Main menu", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Start game",
        MethodToRun = settingsChoice.Run,
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Settings",
        MethodToRun = kindofSettingsToCustomize.Run,
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Load game"
    }
});

while (true)
{
    string? result = mainMenu.Run();

    if (result == "x")
    {
        break;
    }
    else if (result == "m")
    {
        continue;
    }
}