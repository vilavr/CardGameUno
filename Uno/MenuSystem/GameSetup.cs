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
                if (int.TryParse(Console.ReadLine(), out playerCount) && playerCount >= 2 && playerCount <= 10)
                {
                    break;
                }
                Console.WriteLine("Invalid number, please enter a number between 2 and 10.");
            }

            string playerInfos = ""; // Initializing the string that will hold player information.
            Regex validNicknameRegex = new Regex("^[a-zA-Z0-9_-]+$"); // Regex to match allowed characters.
            HashSet<string> existingNicknames = new HashSet<string>(); // Store for already used nicknames.

            for (int i = 0; i < playerCount; i++)
            {
                string nickname;
                while (true) // Loop until a valid, unique nickname is entered.
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
                        existingNicknames.Add(nickname); // Add the new unique nickname to the HashSet.
                        break;
                    }
                }

                var playerType = ChoosePlayerType();

                var player = new Player(_nextPlayerId++, nickname, playerType); // IDs are auto-incremented.

                // Constructing the player information string.
                playerInfos += $"{player.Id} : {player.Nickname} : {player.Type}";

                if (i < playerCount - 1) // Not the last player? Then add a separator.
                {
                    playerInfos += " ; ";
                }

                Console.WriteLine($"{nickname} has been added as a {playerType} player.");
            }

            Console.WriteLine("All players have been created successfully.");
            return playerInfos; // Return the structured string.
        }



        private EPlayerType ChoosePlayerType()
        {
            while (true) // keep asking until a valid input is provided
            {
                Console.WriteLine("Choose player type:");
                Console.WriteLine("1. Human");
                Console.WriteLine("2. AI");
                Console.Write("Enter your choice (1-2): ");

                string choice = Console.ReadLine()!;
        
                switch (choice)
                {
                    case "1":
                        return EPlayerType.Human;
                    case "2":
                        return EPlayerType.AI;
                    default:
                        Console.WriteLine("Invalid selection. Please enter 1 for Human or 2 for AI.");
                        break; // this will continue the loop since the user input was not valid
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
                if (parts.Length == 3) // If there are exactly three parts, then we consider it valid.
                {
                    int id = int.Parse(parts[0]);
                    string nickname = parts[1];
                    EPlayerType type = (EPlayerType)Enum.Parse(typeof(EPlayerType), parts[2]);

                    players.Add(new Player(id, nickname, type));
                }
            }

            return players;
        }
    }