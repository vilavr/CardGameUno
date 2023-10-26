using System.Text.Json;
using System.Linq;

namespace MenuSystem;

public class CardsDeal
{
    private readonly CardDeck _deck;
    private readonly List<Player> _players;
    private readonly int _numberOfCardsPerPlayer;
    private readonly string _jsonFilePath = "/home/viralavrova/cardgameuno/Uno/Resources/carddeal_info.json";

    public CardsDeal(CardDeck deck, List<Player> players, int numberOfCardsPerPlayer)
    {
        _deck = deck;
        _players = players;
        _numberOfCardsPerPlayer = numberOfCardsPerPlayer;
    }

    public void DealCards()
    {
        for (int i = 0; i < _numberOfCardsPerPlayer; i++)
        {
            foreach (var player in _players)
            {
                var card = _deck.DrawCard(); // by default draw from the top
                player.ReceiveCard(card); 
            }
        }
        SaveDealtCardsInfo();
    }

    private void SaveDealtCardsInfo()
    {
        // Constructing the data structure for serialization
        var dataToSerialize = _players.ToDictionary(
            player => player.Nickname,
            player => player.Hand.GroupBy(card => card.ToString())
                .ToDictionary(group => group.Key, group => group.Count())
        );
        
        string jsonString = JsonSerializer.Serialize(dataToSerialize, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_jsonFilePath, jsonString);
    }
}