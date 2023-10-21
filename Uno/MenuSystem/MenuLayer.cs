namespace MenuSystem;

public class MenuLayer
{
    public string Title { get; set; }
    public Dictionary<string, MenuItem> MenuItems { get; set; }
    public string[] ReservedShortcuts { get; set; }

    public MenuLayer(string title, Dictionary<string, MenuItem> menuItems, string[] reservedShortcuts)
    {
        Title = title;
        MenuItems = menuItems;
        ReservedShortcuts = reservedShortcuts;
    }
}