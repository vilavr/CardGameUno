namespace MenuSystem;

public class Menu
{
    public string Title { get; set; } = default!;
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();

    private string Separator = "============================";
    private static readonly string[] ReservedShortcuts = new[] { "x", "b" };

    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        foreach (var menuItem in menuItems)
        {
            if (ReservedShortcuts.Contains(menuItem.Shortcut.ToLower()))
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
        
        Console.WriteLine("b) Back");
        Console.WriteLine("x) Exit");
        
        Console.WriteLine(Separator);
        Console.Write("Your choice: ");
    }

    public string? Run()
    {
        var userChoice = "";
        do
        {
            Console.Clear();
            Draw();
            userChoice = Console.ReadLine()?.Trim();

            if (MenuItems.ContainsKey(userChoice?.ToLower()))
            {
                if (MenuItems[userChoice!.ToLower()].MethodToRun != null)
                {
                    var result = MenuItems[userChoice.ToLower()].MethodToRun!();
                    if (result?.ToLower() == "x")
                    {
                        userChoice = "x";
                    }
                }
                
            }
            else if (!ReservedShortcuts.Contains(userChoice?.ToLower()))
            {
                Console.WriteLine("Unsupported shortcut");
            }
            Console.WriteLine();
        } while (!ReservedShortcuts.Contains(userChoice));
        
        return userChoice;
    }
}