using Newtonsoft.Json.Linq;

namespace MenuSystem;

public class CardDeck
{
    private readonly string _settingsFilePath;

    public CardDeck(string settingsFilePath)
    {
        _settingsFilePath = settingsFilePath;
        Cards = new List<Card>();
    }

    public List<Card> Cards { get; }

    public void PrintDeck()
    {
        Console.WriteLine("Current Deck:");
        if (Cards.Count == 0)
        {
            Console.WriteLine("   [The deck is empty]");
            return;
        }

        foreach (var card in Cards)
            // Assuming the Card class has a method ToString() implemented that describes the card
            Console.WriteLine("   " + card);
    }

    private (CardColor color, CardValue value) GetCardAttributes(string cardKey)
    {
        // Special handling for Wild cards as they don't have the same key pattern as regular cards
        if (cardKey.Equals("Wild", StringComparison.OrdinalIgnoreCase))
            return (CardColor.Wild, CardValue.Wild); // Regular wild card
        if (cardKey.Equals("Wild_DrawFour", StringComparison.OrdinalIgnoreCase))
            return (CardColor.Wild, CardValue.WildDrawFour); // Special Wild Draw Four card

        var parts = cardKey.Split('_');
        if (parts.Length != 2) throw new Exception($"Invalid card key: {cardKey}");

        // Parse the color and value from the key parts.
        if (!Enum.TryParse(parts[0], true, out CardColor color))
            throw new Exception($"Invalid color in card key: {cardKey}");

        if (!Enum.TryParse(parts[1], true, out CardValue value))
            throw new Exception($"Invalid value in card key: {cardKey}");

        return (color, value);
    }

    public void InitializeDeck()
    {
        // Read the settings file and parse the JSON
        var jsonData = File.ReadAllText(_settingsFilePath);
        var jsonObject = JObject.Parse(jsonData);

        // Extract the number of decks and card settings
        var numberOfDecks = (int)jsonObject["gameSettings"]!["NumberOfDecks"];
        var cardSettings = (JObject)jsonObject["cardSettings"]!;

        // Initialize the deck
        foreach (var card in cardSettings.Properties())
        {
            var cardKey = card.Name;
            var cardCount = (int)card.Value;

            // Parse card's color and value from its key
            var (color, value) = GetCardAttributes(cardKey);

            // Add cards to the deck based on the specified count and number of decks
            for (var i = 0; i < cardCount * numberOfDecks; i++) Cards.Add(new Card(color, value));
        }

        PrintDeck(); // Optionally print the deck after initialization
    }
    
    public void ShuffleDeck()
    {
        Random rng = new Random();
        int n = Cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1); 
            (Cards[k], Cards[n]) = (Cards[n], Cards[k]);
        }
        PrintDeck();
    }

}