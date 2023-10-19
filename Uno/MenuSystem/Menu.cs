namespace MenuSystem;

public class Menu
{
    public string Title { get; set; } = default!;
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();

    private string Separator = "============================";
    private static readonly string[] ForbiddenShortcuts = new[] { "x", "b" };

    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        foreach (var menuItem in menuItems)
        {
            if (ForbiddenShortcuts.Contains(menuItem.Shortcut.ToLower()))
            {
                throw new Exception(message: $"The shortcut '{menuItem.Shortcut.ToLower()}' is not allowed!");
            }

            MenuItems[menuItem.Shortcut] = menuItem;
        }
    }
    private void Draw()
    {
        Console.WriteLine(Title);
        Console.WriteLine(Separator);
        foreach (var menuItem in MenuItems.Values)
        {
            Console.Write(menuItem.Shortcut);
            Console.Write(") ");
            Console.WriteLine(menuItem.MenuLabel);
        }
        Console.WriteLine(Separator);
        Console.Write("Your choice: ");
    }

    public string Run()
    {
        Draw();
        return "x";
    }
}