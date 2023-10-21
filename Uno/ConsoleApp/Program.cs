using MenuSystem;


var submenu1 = new Menu(title:"submenu", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Start game",
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Settings"
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Load game"
    }
});
var mainMenu = new Menu(title:"Main menu", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Start game",
        MethodToRun = submenu1.Run,
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Settings"
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Load game"
    }
});

var UserChoice = mainMenu.Run();