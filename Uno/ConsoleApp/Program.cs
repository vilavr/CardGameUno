using MenuSystem;
var submenu3 = new Menu(title:"submenu3", new List<MenuItem>()
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
}, EMenuLevel.Other);
var submenu2 = new Menu(title:"submenu2", new List<MenuItem>()
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
        MenuLabel = "Load game",
        MethodToRun = submenu3.Run
    }
}, EMenuLevel.Other);
var submenu1 = new Menu(title:"submenu1", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Start game",
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Settings",
        MethodToRun = submenu2.Run,
    },
    new MenuItem()
    {
        Shortcut = "3",
        MenuLabel = "Load game"
    }
}, EMenuLevel.Second);
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