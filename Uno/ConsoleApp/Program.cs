using MenuSystem;
var directionPrompt = new Prompt<string>(
    "Enter playing direction (clockwise or counterclockwise): ", 
    new List<string> { "clockwise", "counterclockwise" }
);
var gameSettingsToCustomize = new Menu(title:"Choose what category of settings you'd like to customize", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Number of decks",
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Winning score",
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
        MenuLabel = "Card settings"
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Game settings",
        MethodToRun = gameSettingsToCustomize.Run
    }
}, EMenuLevel.Other);

var whatToDoWithSettings = new Menu(title:"Choose what you want to do with these settings", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Use only for this game (without saving)",
        MethodToRun = kindofSettingsToCustomize.Run,
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Use for this game and save into file",
        MethodToRun = kindofSettingsToCustomize.Run,
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Use for this game and set as default for further games",
        MethodToRun = kindofSettingsToCustomize.Run,
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
        MethodToRun = whatToDoWithSettings.Run,
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
        MethodToRun = whatToDoWithSettings.Run,
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