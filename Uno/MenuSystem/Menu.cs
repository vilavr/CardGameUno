namespace MenuSystem;

public class Menu
{
    public string Title { get; set; } = default!;
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();
    public EMenuLevel MenuLevel { get; set; }
    
    private string Separator = "============================";
    private static readonly string[] ReservedShortcutsFirst = new[] { "x" };
    private static readonly string[] ReservedShortcutsSecond = new[] { "x", "b" };
    private static readonly string[] ReservedShortcutsThird = new[] { "x", "b", "m" };
    private string[] ReservedShortcuts;
    public void RefreshAvailableShortcuts()
    {
        switch (MenuLevel)
        {
            case EMenuLevel.First:
                ReservedShortcuts = ReservedShortcutsFirst;
                break;
            case EMenuLevel.Second:
                ReservedShortcuts = ReservedShortcutsSecond;
                break;
            case EMenuLevel.Other:
                ReservedShortcuts = ReservedShortcutsThird;
                break;
        }
    }
    public bool ShouldReturnToMain { get; private set; } = false;
    public Menu(string? title, List<MenuItem> menuItems, EMenuLevel menuLevel = EMenuLevel.First)
    {
        Title = title;
        MenuLevel = menuLevel; 
        ReservedShortcuts = ReservedShortcutsFirst;
        RefreshAvailableShortcuts();
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
        RefreshAvailableShortcuts();
        foreach (var menuItem in MenuItems.Values)
        {
            Console.Write(menuItem.Shortcut);
            Console.Write(") ");
            Console.WriteLine(menuItem.MenuLabel);
        }

        if (MenuLevel != EMenuLevel.First)
        { 
            Console.WriteLine("b) Back");
            if (MenuLevel != EMenuLevel.Second)
            {
                Console.WriteLine("m) Return to main");
            }
        }

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
        userChoice = Console.ReadLine()?.Trim().ToLower() ?? "";
        if (MenuItems.ContainsKey(userChoice))
        {
            var selectedMenuItem = MenuItems[userChoice];
            if (selectedMenuItem.MethodToRun != null)
            {
                var result = selectedMenuItem.MethodToRun();
                if (result?.ToLower() == "x")
                {
                    return "x"; 
                }
                else if (result?.ToLower() == "m")  
                {
                    return "m"; 
                }
            }
        }
        else if (userChoice == "b")  // User wants to go back one level
        {
            if (MenuLevel != EMenuLevel.First)
            {
                return "b"; 
            }
        }
        else if (userChoice == "m")  // User wants to jump back to the main menu
        {
            if (MenuLevel != EMenuLevel.First)
            {
                return "m"; 
            }
        }
        else if (userChoice == "x")  // User wants to exit the application
        {
            return "x";
        }
        else  // The choice did not match any item or reserved shortcut
        {
            Console.WriteLine("Unsupported shortcut. Press any key to try again.");
            Console.ReadKey();
        }

    } while (true); 
}

}