namespace MenuSystem;

public class MenuController
{
    private Stack<Menu> MenuHistory = new Stack<Menu>();

    public void DisplayMenu(Menu menu)
    {
        MenuHistory.Push(menu);  // Add the current menu to the stack

        string? choice;
        do
        {
            choice = menu.Run(); 
            if (choice == "b" && MenuHistory.Count > 1)
            {
                MenuHistory.Pop();  // Remove the current menu from the stack
                // Display the previous menu
                DisplayMenu(MenuHistory.Peek());
            }
            else if (menu.ShouldReturnToMain)
            {
                while (MenuHistory.Count > 1)  // Remove all sub-menus from the stack
                {
                    MenuHistory.Pop();
                }
                DisplayMenu(MenuHistory.Peek());
            }
        } while (choice != "x");  
    }
}
