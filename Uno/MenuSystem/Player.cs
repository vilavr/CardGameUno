using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MenuSystem;

public class Player
{
    public Player(int id, string nickname, EPlayerType type)
    {
        Id = id;
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
        Type = type;
        Hand = new List<Card>();
    }

    public int Id { get; }
    public string Nickname { get; set; }
    public EPlayerType Type { get; set; }

    // This list holds the cards that the player has.
    public List<Card> Hand { get; }

    public void ReceiveCard(Card card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card), "Cannot add a null card to a player's hand.");

        Hand.Add(card);
    }

    public IReadOnlyList<Card> ShowHand()
    {
        return Hand.AsReadOnly();
    }

    public void TakeCard(Card card, string jsonFilePath)
    {
        Hand.Add(card);
        UpdatePlayerHandInJson(jsonFilePath, card, true);
    }

    public bool GetRidOfCard(Card card, string jsonFilePath)
    {
        var removed = Hand.Remove(card);
        if (removed) UpdatePlayerHandInJson(jsonFilePath, card, false);
        return removed;
    }

    private void UpdatePlayerHandInJson(string jsonFilePath, Card card, bool isTaking)
    {
        var jsonData = File.ReadAllText(jsonFilePath);
        var playersData = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(jsonData);

        if (playersData != null && playersData.TryGetValue(Id.ToString(), out var playerData))
        {
            var hand = playerData["Hand"]?.ToObject<Dictionary<string, int>>();
            if (hand == null) throw new InvalidOperationException("The player's hand data is missing or invalid.");

            var cardKey = card.ToString();

            if (isTaking)
            {
                if (hand.ContainsKey(cardKey))
                    hand[cardKey]++;
                else
                    hand[cardKey] = 1;
            }
            else // Getting rid of a card
            {
                if (hand.ContainsKey(cardKey) && hand[cardKey] > 0)
                {
                    if (hand[cardKey] > 1)
                        hand[cardKey]--;
                    else
                        hand.Remove(cardKey); // Or remove the card entry if the count drops to 0.
                }
                else
                {
                    throw new InvalidOperationException("The specified card does not exist in the player's hand.");
                }
            }

            // Update the player's data with the modified hand.
            playerData["Hand"] = JObject.FromObject(hand);
            playersData[Id.ToString()] = playerData;

            var outputJson = JsonConvert.SerializeObject(playersData, Formatting.Indented);
            File.WriteAllText(jsonFilePath, outputJson);
        }
        else
        {
            throw new KeyNotFoundException($"Player with ID {Id} not found.");
        }
    }
}