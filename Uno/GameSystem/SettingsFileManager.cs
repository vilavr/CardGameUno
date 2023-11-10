namespace GameSystem;

public class SettingsFileManager
{
    private readonly string _resourcesDirectory;

    public SettingsFileManager(string resourcesDirectory)
    {
        _resourcesDirectory = resourcesDirectory;
    }

    public void PromptAndCopySettings(bool isDefault)
    {
        if (isDefault)
        {
            var defaultFilePath = Path.Combine(_resourcesDirectory, "default_settings.json");
            if (File.Exists(defaultFilePath))
            {
                CopyContentsToSettingsInfo(defaultFilePath);
                return; // Early return after copying from the default settings
            }

            Console.WriteLine("Default settings file not found.");
            return; // If the default file is missing, we can't proceed further
        }

        // Define filenames to exclude
        var excludedFilenames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "settings_info.json",
            "players_info.json",
            "default_settings.json"
        };
        var files = Directory.GetFiles(_resourcesDirectory, "*.json")
            .Where(file => !excludedFilenames.Contains(Path.GetFileName(file))) // Filter out excluded files
            .ToArray();

        if (files.Length == 0)
        {
            Console.WriteLine("No settings files available.");
            return;
        }

        Console.WriteLine("Available settings files:");
        foreach (var file in files) Console.WriteLine($"{Path.GetFileName(file)}");

        while (true) // Keep prompting the user until a valid input is received
        {
            Console.Write(
                "Enter the name of the settings file you want to use (filename.json) or press 'f' to finish: ");
            var input = Console.ReadLine()?.Trim();
            if (string.Equals(input, "f", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Operation canceled by the user.");
                return;
            }
            if (string.IsNullOrWhiteSpace(input) || excludedFilenames.Contains(input) ||
                !input.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "Invalid input. Please enter a valid file name with a .json extension or 'f' to finish.");
                continue;
            }

            var selectedFilePath = Path.Combine(_resourcesDirectory, input);
            if (File.Exists(selectedFilePath))
            {
                // Valid file selected, proceed to copy the contents
                CopyContentsToSettingsInfo(selectedFilePath);
                break;
            }

            Console.WriteLine($"File not found: {input}");
        }
    }


    public void CopyContentsToSettingsInfo(string sourceFilePath, string destinationFileName = "settings_info.json")
    {
        // Define the full path for the destination file within the resources directory
        var destinationFilePath = Path.Combine(_resourcesDirectory, destinationFileName);

        try
        {
            var fileContents = File.ReadAllText(sourceFilePath);
            File.WriteAllText(destinationFilePath, fileContents);

            Console.WriteLine($"Settings have been successfully copied to {destinationFileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while copying settings: {ex.Message}");
        }
    }
}