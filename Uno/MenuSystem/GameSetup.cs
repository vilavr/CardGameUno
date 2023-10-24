using System.Text.Json;
using System.Text.RegularExpressions;

namespace MenuSystem;

public class GameSetup
{
    private int _nextPlayerId = 1; // Used for auto-incrementing player IDs

    public string CreatePlayers()
    {
        Console.WriteLine("Enter the number of players (between 2 and 10):");
        int playerCount;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out playerCount) && playerCount >= 2 && playerCount <= 10) break;
            Console.WriteLine("Invalid number, please enter a number between 2 and 10.");
        }

        var playerInfos = "";
        var validNicknameRegex = new Regex("^[a-zA-Z0-9_-]+$");
        var existingNicknames = new HashSet<string>();

        for (var i = 0; i < playerCount; i++)
        {
            string nickname;
            while (true)
            {
                Console.WriteLine($"Enter nickname for player {i + 1}:");
                nickname = Console.ReadLine()?.Trim() ?? "Unnamed";

                if (!validNicknameRegex.IsMatch(nickname))
                {
                    Console.WriteLine("Invalid nickname. Only letters, numbers, hyphens, and underscores are allowed.");
                }
                else if (existingNicknames.Contains(nickname))
                {
                    Console.WriteLine("This nickname is already taken. Please enter a unique nickname.");
                }
                else
                {
                    existingNicknames.Add(nickname);
                    break;
                }
            }

            var playerType = ChoosePlayerType();

            var player = new Player(_nextPlayerId++, nickname, playerType);

            playerInfos += $"{player.Id} : {player.Nickname} : {player.Type}";

            if (i < playerCount - 1)
                playerInfos += " ; ";

            Console.WriteLine($"{nickname} has been added as a {playerType} player.");
        }

        Console.WriteLine("All players have been created successfully.");
        var players = ParsePlayerInfo(playerInfos);
        ReviewAndEditPlayers(players);
        return playerInfos;
    }


    private EPlayerType ChoosePlayerType()
    {
        while (true)
        {
            Console.WriteLine("Choose player type:");
            Console.WriteLine("1. Human");
            Console.WriteLine("2. AI");
            Console.Write("Enter your choice (1-2): ");

            var choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1":
                    return EPlayerType.Human;
                case "2":
                    return EPlayerType.AI;
                default:
                    Console.WriteLine("Invalid selection. Please enter 1 for Human or 2 for AI.");
                    break;
            }
        }
    }

    public List<Player> ParsePlayerInfo(string playerInfo)
    {
        var players = new List<Player>();

        // Split the input string into individual player info strings.
        var playerInfos = playerInfo.Split(new[] { " ; " }, StringSplitOptions.None);

        foreach (var info in playerInfos)
        {
            // Split each player's info into its components.
            var parts = info.Split(new[] { " : " }, StringSplitOptions.None);
            if (parts.Length == 3)
            {
                var id = int.Parse(parts[0]);
                var nickname = parts[1];
                var type = (EPlayerType)Enum.Parse(typeof(EPlayerType), parts[2]);

                players.Add(new Player(id, nickname, type));
            }
        }

        return players;
    }

    public void ReviewAndEditPlayers(List<Player> players)
    {
        while (true) // Loop until the user is satisfied with the player list
        {
            Console.WriteLine("\nList of players:");
            for (var i = 0; i < players.Count; i++)
                Console.WriteLine($"{players[i].Id}. {players[i].Nickname}, {players[i].Type}");

            Console.WriteLine(
                "\nPress ENTER if you are satisfied with the list, or enter the player's ID you wish to edit:");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                // Save the finalized list of players to the JSON file.
                SavePlayersToJson(players);
                Console.WriteLine("Players have been saved successfully.");
                break;
            }

            if (int.TryParse(userInput, out var playerId) && playerId > 0 && playerId <= players.Count)
            {
                var playerToEdit = players.Find(p => p.Id == playerId);
                if (playerToEdit != null)
                {
                    Console.WriteLine($"Editing details for player {playerId} ({playerToEdit.Nickname})");
                    var newNickname = GetUniqueNickname(players, playerToEdit.Nickname);
                    var newPlayerType = ChoosePlayerType();

                    // Update the player's details.
                    playerToEdit.Nickname = newNickname;
                    playerToEdit.Type = newPlayerType;

                    Console.WriteLine($"{playerToEdit.Nickname} has been updated to be a {playerToEdit.Type} player.");
                }
            }
            else
            {
                Console.WriteLine("Invalid player ID. Please enter a correct player ID.");
            }
        }
    }

    private string GetUniqueNickname(List<Player> players, string currentNickname)
    {
        var validNicknameRegex = new Regex("^[a-zA-Z0-9_-]+$");
        string newNickname;
        bool isUnique;

        do
        {
            isUnique = true;
            Console.WriteLine("Enter a new unique nickname:");
            newNickname = Console.ReadLine()?.Trim() ?? "Unnamed";

            if (!validNicknameRegex.IsMatch(newNickname))
            {
                Console.WriteLine("Invalid nickname. Only letters, numbers, hyphens, and underscores are allowed.");
                isUnique = false;
            }
            else if (newNickname.Equals(currentNickname, StringComparison.OrdinalIgnoreCase))
            {
                // If the nickname is the same as the current, it's considered unique
                break;
            }
            else
            {
                // Check if any other player already has the chosen nickname.
                foreach (var player in players)
                    if (player.Nickname.Equals(newNickname, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("This nickname is already taken. Please enter a unique nickname.");
                        isUnique = false;
                        break;
                    }
            }
        } while (!isUnique);

        return newNickname;
    }

    public void SavePlayersToJson(List<Player> players)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(players, options);
        var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "/home/viralavrova/cardgameuno/Uno/Resources");
        Directory.CreateDirectory(directoryPath); // If it already exists, this method does nothing
        var filePath = Path.Combine(directoryPath, "players_info.json");
        File.WriteAllText(filePath, json);
    }
}