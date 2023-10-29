using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MenuSystem;

public class GameSaver
{
    private readonly string
        _playersInfoPath = "/home/viralavrova/cardgameuno/Uno/Resources/players_info.json"; // Path to players info

    private readonly string
        _settingsInfoPath = "/home/viralavrova/cardgameuno/Uno/Resources/settings_info.json"; // Path to game settings

    public void SaveGame(GameState gameState)
    {
        Console.WriteLine("Please enter a file name to save the current game state (default: gamestate_info.json):");
        var inputFileName = Console.ReadLine()!.Trim();

        var targetDirectory = "/home/viralavrova/cardgameuno/Uno/Resources/";

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        var gameStateFilePath = Path.Combine(targetDirectory,
            string.IsNullOrEmpty(inputFileName) ? "gamestate_info.json" : inputFileName);

        var playersInfo = File.ReadAllText(_playersInfoPath);
        var settingsInfo = File.ReadAllText(_settingsInfoPath);

        var gameStateInfo = new
        {
            Players = JsonConvert.DeserializeObject(playersInfo),
            Settings = JsonConvert.DeserializeObject(settingsInfo),
            Game = new
            {
                gameState.AvailableCardsInDeck,
                gameState.CardsInDiscard,
                TopCard = gameState.CurrentTopCard,
                gameState.SpecialCardEffectApplied,
                gameState.CurrentPlayerTurn,
                gameState.CurrentRound
            }
        };

        var jsonToWrite = JsonConvert.SerializeObject(gameStateInfo, Formatting.Indented);

        try
        {
            File.WriteAllText(gameStateFilePath, jsonToWrite);
            Console.WriteLine($"Game state saved successfully to \"{gameStateFilePath}\".");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving the game: {ex.Message}");
        }
    }


    private readonly string _directoryPath = "/home/viralavrova/cardgameuno/Uno/Resources";

    public bool PromptUserForLoad(out string selectedFilePath)
    {
        var saveFiles = Directory.GetFiles(_directoryPath, "*.json");

        if (saveFiles.Length == 0)
        {
            Console.WriteLine("No save files found!");
            selectedFilePath = string.Empty;
            return false;
        }

        Console.WriteLine("Available save files:");
        for (int i = 0; i < saveFiles.Length; i++)
        {
            Console.WriteLine($"{i + 1}: {Path.GetFileName(saveFiles[i])}");
        }

        Console.WriteLine("Enter the number of the file you want to load:");
        int fileNumber;
        bool isValidInput = int.TryParse(Console.ReadLine(), out fileNumber);

        if (!isValidInput || fileNumber < 1 || fileNumber > saveFiles.Length)
        {
            Console.WriteLine("Invalid selection.");
            selectedFilePath = string.Empty;
            return false;
        }

        selectedFilePath = saveFiles[fileNumber - 1];
        return true;
    }

    public GameState? LoadGame(string filePath)
    {
        Console.WriteLine($"Attempting to load game from \"{filePath}\"...");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: The specified file does not exist.");
            return null;
        }

        string fileContent;
        try
        {
            fileContent = File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            return null;
        }

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            Console.WriteLine("Error: The file is empty.");
            return null;
        }

        Console.WriteLine("File read successfully. Attempting to deserialize the content...");

        dynamic? loadedData = null;
        try
        {
            loadedData = JsonConvert.DeserializeObject(fileContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during deserialization: {ex.Message}");
            return null;
        }

        if (loadedData == null)
        {
            Console.WriteLine("Failed to deserialize the game data. The save file might be corrupted.");
            return null;
        }

        Console.WriteLine("Deserialization successful. Extracting game segments...");

        // First, write the content of 'Players' and 'Settings' to their respective files
        try
        {
            string playersInfo = JsonConvert.SerializeObject(loadedData.Players);
            File.WriteAllText(_playersInfoPath,
                playersInfo); 

            string settingsInfo = JsonConvert.SerializeObject(loadedData.Settings);
            File.WriteAllText(_settingsInfoPath,
                settingsInfo); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while extracting segments: {ex.Message}");
            return null;
        }

        Console.WriteLine("Segments extracted. Preparing to construct game state...");

        try
        {
            // Parse 'Players' from the 'Players' segment

            var playersJson = JsonConvert.SerializeObject(loadedData.Players);
            // var settingsJson = JsonConvert.SerializeObject(loadedData.Settings);

            if (string.IsNullOrWhiteSpace(playersJson))
            {
                Console.WriteLine("Error: No players' information available.");
                return null;
            }

            Console.WriteLine("Segments extracted. Preparing to construct game state...");

            var deserializedPlayers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(playersJson);
            if (deserializedPlayers == null)
            {
                Console.WriteLine("Error: Could not parse players' information.");
                return null;
            }

            var players = new List<Player>();
            foreach (var kvp in deserializedPlayers)
            {
                string playerIdString = kvp.Key;
                dynamic playerData = kvp.Value;

                int playerId = int.Parse(playerIdString);
                string nickname = playerData.Nickname;
                EPlayerType playerType = (EPlayerType)Enum.Parse(typeof(EPlayerType), (string)playerData.Type);

                var player = new Player(playerId, nickname, playerType)
                {
                    Score = playerData.Score
                };

                // Convert hand cards
                var handCards = new List<Card>();
                foreach (var cardEntry in playerData.Hand)
                {
                    string cardName = cardEntry.Name; 
                    int cardQuantity = (int)cardEntry.Value; 
                    
                    for (int i = 0; i < cardQuantity; i++)
                    {
                        Card card = CreateCardFromName(cardName); 
                        handCards.Add(card);
                    }
                }

                player.Hand = handCards; 
                players.Add(player);
            }

            GameState gameState = new GameState
            {
                AvailableCardsInDeck = loadedData.Game.AvailableCardsInDeck.ToObject<List<Card>>(),
                CardsInDiscard = loadedData.Game.CardsInDiscard.ToObject<List<Card>>(),
                CurrentTopCard = loadedData.Game.TopCard.ToObject<Card>(),
                SpecialCardEffectApplied = loadedData.Game.SpecialCardEffectApplied,
                CurrentPlayerTurn = loadedData.Game.CurrentPlayerTurn,
                CurrentRound = loadedData.Game.CurrentRound,
                Players = players 
            };

            Console.WriteLine($"Game state created successfully with {gameState.Players?.Count} player(s).");
            Console.WriteLine($"Game loaded from \"{Path.GetFileName(filePath)}\".");

            // gameState.PrintGameState(gameState);
            return gameState;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while constructing the game state: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return null;
        }
    }


    private List<Card> ConvertHandToList(Dictionary<string, int> handDict)
    {
        var cardList = new List<Card>();
        foreach (var item in handDict)
        {
            var cardName = item.Key;
            var quantity = item.Value;

            for (int i = 0; i < quantity; i++)
            {
                var card = CreateCardFromName(cardName);
                cardList.Add(card);
            }
        }
        return cardList;
    }
    
    private Card CreateCardFromName(string cardName)
    {
        string[] parts = cardName.Split(' ');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Card name is not in the expected format.");
        }

        CardColor cardColor;
        if (!Enum.TryParse(parts[0], true, out cardColor))
        {
            throw new ArgumentException("Invalid color in card name.");
        }

        CardValue cardValue;
        if (!Enum.TryParse(parts[1], true, out cardValue))
        {
            string adjustedValue = Regex.Replace(parts[1], "(\\B[A-Z])", " $1");
            if (!Enum.TryParse(adjustedValue, true, out cardValue))
            {
                throw new ArgumentException("Invalid value in card name.");
            }
        }

        return new Card(cardColor, cardValue);
    }

}

