namespace MenuSystem;

public class GameState
{
    public int Score { get; set; }
    public List<Card> AvailableCardsInDeck { get; set; }
    public List<Card> CardsInDiscard { get; set; }
    public Card? CurrentTopCard { get; set; }
    public int CurrentPlayerTurn { get; set; }

    public GameState()
    {
        // Initialize the lists to prevent null reference issues.
        AvailableCardsInDeck = new List<Card>();
        CardsInDiscard = new List<Card>();
    }

    public void SetInitialTopCard(Card topCard)
    {
        CurrentTopCard = topCard;
        CardsInDiscard.Add(topCard);
    }

}