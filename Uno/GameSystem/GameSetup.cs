using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Domain;

namespace GameSystem;

public class GameSetup
{
    private int _nextPlayerId = 1; // Used for auto-incrementing player IDs

    public List<Player> CreatePlayers()
    {
        Console.WriteLine("Enter the number of players (between 2 and 10):");
        int playerCount;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out playerCount) && playerCount >= 2 && playerCount <= 10) break;
            Console.WriteLine("Invalid number, please enter a number between 2 and 10.");
        }

        var players = new List<Player>();
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
            players.Add(player);

            Console.WriteLine($"{nickname} has been added as a {playerType} player.");
        }

        Console.WriteLine("All players have been created successfully.");

        ReviewAndEditPlayers(players);
        return players;
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
    public void PrintPlayersList(List<Player> players)
    {
        Console.WriteLine("\nList of players:");
        for (var i = 0; i < players.Count; i++)
        {
            // Print each player's details in the specified format
            Console.WriteLine($"{players[i].Id}. {players[i].Nickname}, {players[i].Type}");
        }
    }

    public void ReviewAndEditPlayers(List<Player> players)
    {
        while (true) // Loop until the user is satisfied with the player list
        {
            PrintPlayersList(players);

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
                    
                    SavePlayersToJson(players);
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
    public Dictionary<string, int> PreparePlayerHandForSerialization(List<Card> hand)
    {
        var cardCounts = new Dictionary<string, int>();

        foreach (var card in hand)
        {
            var cardName = card.ToString();

            if (cardCounts.ContainsKey(cardName))
            {
                cardCounts[cardName]++;
            }
            else
            {
                cardCounts[cardName] = 1;
            }
        }

        return cardCounts;
    }
    public void SavePlayersToJson(List<Player> players)
    {
        var playerData = new Dictionary<string, PlayerInfo>();

        foreach (var player in players)
        {
            // Prepare the hand for serialization as before
            var preparedHand = PreparePlayerHandForSerialization(player.Hand);
            // Create a new PlayerInfo record with the required information
            var info = new PlayerInfo(player.Nickname, player.Type.ToString(), preparedHand, player.Score); 
            
            // Add this to our dictionary
            playerData[player.Id.ToString()] = info;
        }

        // Modify JsonSerializerOptions to serialize enums as strings
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() } // This converter is necessary for handling enums as strings
        };

        var json = JsonSerializer.Serialize(playerData, options);

        var directoryPath = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../")), "Resources");
        Directory.CreateDirectory(directoryPath); // If it already exists, this method does nothing
        var filePath = Path.Combine(directoryPath, "players_info.json");
        File.WriteAllText(filePath, json);
    }

    
    public List<Player> SitPlayers(List<Player> players, GameSettings settings)
    {
        // Load the JSON configuration from the file
        var jsonFilePath = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../")), "Resources/settings_info.json");
        var jsonString = File.ReadAllText(jsonFilePath);

        GameSettings gameSettings;
        try
        {
            gameSettings = new GameSettings(jsonString);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error in loading game settings: {ex.Message}");
            return players; 
        }

        var isCounterclockwise = gameSettings.PlayDirection.Equals("counterclockwise", StringComparison.OrdinalIgnoreCase);
        if (isCounterclockwise)
        {
            players.Reverse();
        }
        SavePlayersToJson(players);
        return players;
    }
    
    public void DetermineFirstPlayerAndReorder(List<Player> players)
    {
        if (players == null || players.Count == 0)
        {
            throw new InvalidOperationException("There must be at least one player to start the game.");
        }

        // Randomly select an ID. IDs range from 1 to players.Count.
        var random = new Random();
        int randomPlayerId = random.Next(1, players.Count + 1); // This generates a number between 1 and players.Count inclusive.

        // Find the player with the selected ID.
        Player firstPlayer = players.FirstOrDefault(p => p.Id == randomPlayerId)!;

        if (firstPlayer == null)
        {
            throw new InvalidOperationException("No player found with the selected ID.");
        }
        var reorderedPlayers = new List<Player> { firstPlayer };
        reorderedPlayers.AddRange(players.Where(p => p.Id != randomPlayerId));
        players.Clear();
        players.AddRange(reorderedPlayers);
        SavePlayersToJson(players);

        Console.WriteLine($"{firstPlayer.Nickname} has been randomly selected to start the game.");
    }
    
    public int GetCurrentPlayerId(List<Player> _players)
    {
        return _players[0].Id;
    }
    public void AdvanceTurn(List<Player> _players, GameState gameState)
    {
        int currentIndex = _players.FindIndex(p => p.Id == gameState.CurrentPlayerTurn);
        int nextIndex = (currentIndex + 1) % _players.Count;
        gameState.CurrentPlayerTurn = _players[nextIndex].Id;
        SavePlayersToJson(_players); 
    }



    public void ReversePlayerOrder(List<Player> _players, ref GameState gameState)
    {
        _players.Reverse();
        AdvanceTurn(_players, gameState); 
        SavePlayersToJson(_players);
    }
}