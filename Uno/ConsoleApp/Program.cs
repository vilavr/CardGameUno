using MenuSystem;

var mainMenu = new Menu(title:"Main menu", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "1",
        MenuLabel = "Item 1",
    },
    new MenuItem()
    {
        Shortcut = "2",
        MenuLabel = "Item 2"
    }
});

var UserChoice = mainMenu.Run();