using MenuSystem;
var choiceOfSettingToCustomise = new Menu(title:"Choose what setting you'd like to customize", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Setting 1",
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Setting 2"
    },
    new MenuItem()
    {
        Shortcut = "f",
        MenuLabel = "Finish customisation"
    }
}, EMenuLevel.Other);
var whatToDoWithSettings = new Menu(title:"Choose what you want to do with these settings", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Use only for this game (without saving)",
        MethodToRun = choiceOfSettingToCustomise.Run,
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Use for this game and save into file",
        MethodToRun = choiceOfSettingToCustomise.Run,
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Use for this game and set as default for further games",
        MethodToRun = choiceOfSettingToCustomise.Run,
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