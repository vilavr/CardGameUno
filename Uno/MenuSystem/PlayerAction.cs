namespace MenuSystem;

public class PlayerAction
{
    private readonly GameState _gameState;
    private readonly Player _player;

    public PlayerAction(Player player, GameState gameState)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
    }

    public bool IsTurnOver { get; private set; }

    public void TakeTurn()
    {
        Console.WriteLine($"Current Top Card: {_gameState.CurrentTopCard}");
        Console.WriteLine($"It's {_player.Nickname}'s turn");

        if (_player.Type == EPlayerType.Human)
            HumanPlayerTurn();
        else
            AIPlayerTurn();
    }

    private void HumanPlayerTurn()
    {
        var validMoveMade = false;
        var cardDrawn = false;

        while (!validMoveMade && !IsTurnOver)
        {
            var hand = _player.ShowHand();
            Console.WriteLine("Your cards:");
            for (var i = 0; i < hand.Count; i++)
                Console.WriteLine(
                    $"{i + 1}: {hand[i]}"); 

            Console.WriteLine(
                "Actions: 'play' to play a card, 'draw' to draw from the deck, or 'pass' to skip your turn.");
            var action = Console.ReadLine()?.Trim().ToLower();

            switch (action)
            {
                case "play":
                    Console.WriteLine("Choose a card number to play:");
                    var cardInput = Console.ReadLine()!;

                    if (int.TryParse(cardInput, out var cardIndex) && cardIndex >= 1 && cardIndex <= hand.Count)
                    {
                        // Attempt to play the card
                        var selectedCard = hand[cardIndex - 1];
                        if (IsValidMove(
                                selectedCard)) // Make sure IsValidMove checks the card against the current game state.
                        {
                            ExecuteMove(selectedCard,
                                _gameState); // This should handle playing the card, updating the game state, etc.
                            validMoveMade = true;
                        }
                        else
                        {
                            Console.WriteLine("You cannot play that card. Choose a valid move.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid card number.");
                    }

                    break;

                case "draw":
                    if (!cardDrawn)
                    {
                        var newCard =
                            Card.DrawCard(_gameState
                                .AvailableCardsInDeck); // Assuming you moved the DrawCard method to the Card class.

                        if (newCard != null) // Check if drawing was successful based on your deck's state.
                        {
                            _player.Hand.Add(newCard);
                            Console.WriteLine($"You drew a card: {newCard}"); // Display the card that was drawn.


                            if (IsValidMove(newCard)) // Check if the new card can be played.
                            {
                                Console.WriteLine("The card you drew is playable. Do you want to play it? (yes/no)");
                                if (Console.ReadLine()?.Trim().ToLower() == "yes")
                                {
                                    ExecuteMove(newCard, _gameState); // Play the card.
                                    validMoveMade = true;
                                }
                                else
                                {
                                    Console.WriteLine("You kept the card. Your turn is over.");
                                    IsTurnOver = true; // Set that the player's turn is over.
                                }
                            }
                            else
                            {
                                Console.WriteLine("The card you drew isn't playable. Your turn is over.");
                                IsTurnOver = true; // Set that the player's turn is over.
                            }

                            cardDrawn = true; // Prevent drawing more cards this turn.
                        }
                        else
                        {
                            Console.WriteLine("No cards left to draw. You must pass.");
                            IsTurnOver = true; // If no cards are left, the player's turn ends.
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can only draw once per turn.");
                    }

                    break;

                case "pass":
                    Console.WriteLine("You decided to pass your turn.");
                    IsTurnOver = true; // The player's turn is over.
                    break;

                default:
                    Console.WriteLine("Invalid action. Please type 'play', 'draw', or 'pass'.");
                    break;
            }
        }
    }


    private void AIPlayerTurn()
    {
        // Simplified logic for AI to play the first valid card they have
        var validCard = _player.Hand.FirstOrDefault(IsValidMove);

        if (validCard != null)
        {
            Console.WriteLine($"{_player.Nickname} (AI) played {validCard}.");
            ExecuteMove(validCard, _gameState);
        }
        else
        {
            Console.WriteLine($"{_player.Nickname} (AI) skipped their turn.");
            // Implement logic if AI needs to draw a card or take any other action when it can't play
        }

        if (!_player.Hand.Any(card => IsValidMove(card)) &&
            !_gameState.AvailableCardsInDeck.Any()) // Directly check if the deck is empty.
        {
            Console.WriteLine($"AI {_player.Nickname} exhausted all the valid cards");
            IsTurnOver = true;
        }
    }

    private bool IsValidMove(Card card)
    {
        return card.Color == _gameState.CurrentTopCard?.Color || card.Value == _gameState.CurrentTopCard?.Value;
    }

    private void ExecuteMove(Card card, GameState gameState)
    {
        // This method should handle the act of playing a card, updating the game state, and any other consequences of the move.
        _player.GetRidOfCard(card, "/home/viralavrova/cardgameuno/Uno/Resources/players_info.json",
            gameState); 
        _gameState.AddCardToDiscard(
            card); 
        if (!_player.Hand.Any()) Console.WriteLine($"{_player.Nickname} wins the game!");
    }
}