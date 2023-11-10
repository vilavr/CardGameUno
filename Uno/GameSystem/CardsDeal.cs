using System.Text.Json;
using System.Linq;
using Domain;

namespace GameSystem;

public class CardsDeal
{
    private readonly CardDeck _deck;
    private readonly List<Player> _players;
    private readonly int _numberOfCardsPerPlayer;
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
                var card = _deck.DrawCard(); 
                player.ReceiveCard(card); 
            }
        }
    }
}