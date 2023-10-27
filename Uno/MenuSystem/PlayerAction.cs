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

        // Check if there are any playable cards in the player's hand
        var hasPlayableCards = _player.ShowHand().Any(card => IsValidMove(card));

        while (!validMoveMade && !IsTurnOver)
        {
            var hand = _player.ShowHand();
            Console.WriteLine("Your cards:");
            for (var i = 0; i < hand.Count; i++) Console.WriteLine($"{i + 1}: {hand[i]}");

            if (hasPlayableCards)
                Console.WriteLine(
                    "Actions: 'play' to play a card, 'draw' to draw from the deck, or 'pass' to skip your turn.");
            else
                Console.WriteLine("You have no playable cards. You must 'draw' from the deck or 'pass' your turn.");

            var action = Console.ReadLine()?.Trim().ToLower();

            switch (action)
            {
                case "play":
                    if (!hasPlayableCards)
                    {
                        Console.WriteLine("You don't have any playable cards. You need to draw.");
                        break;
                    }

                    Console.WriteLine("Choose a card number to play:");
                    var cardInput = Console.ReadLine();

                    if (int.TryParse(cardInput, out var cardIndex) && cardIndex >= 1 && cardIndex <= hand.Count)
                    {
                        var selectedCard = hand[cardIndex - 1];
                        if (IsValidMove(
                                selectedCard))
                        {
                            ExecuteMove(selectedCard,
                                _gameState);
                            validMoveMade = true;
                            IsTurnOver = true;
                        }
                        else
                        {
                            Console.WriteLine("You cannot play that card. Choose a valid move.");
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            "Invalid card number. Please select a valid number corresponding to your cards.");
                    }

                    break;
                    case "draw":
                    if (!cardDrawn)
                    {
                        if (hasPlayableCards)
                        {
                            Console.WriteLine("You have playable cards. Are you sure you want to draw? (yes/no)");
                            if (Console.ReadLine()?.Trim().ToLower() !=
                                "yes")
                                continue;
                        }

                        var newCard = Card.DrawCard(_gameState.AvailableCardsInDeck); 
                        
                        if (newCard != null)
                        {
                            _player.Hand.Add(newCard);
                            Console.WriteLine($"You drew a card: {newCard}");

                            if (IsValidMove(newCard)) // check if the new card can be played immediately
                            {
                                Console.WriteLine("The card you drew is playable. Do you want to play it? (yes/no)");
                                if (Console.ReadLine()?.Trim().ToLower() == "yes")
                                    ExecuteMove(newCard, _gameState); 
                                else
                                    Console.WriteLine("You chose to keep the card.");

                                IsTurnOver = true;
                                validMoveMade =
                                    true;
                            }
                            else
                            {
                                Console.WriteLine("The card you drew isn't playable. Your turn is over.");
                                IsTurnOver = true;
                            }

                            cardDrawn = true; 
                        }
                        else
                        {
                            Console.WriteLine("No cards left to draw. You must pass.");
                            IsTurnOver = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can only draw once per turn.");
                    }

                    break;


                case "pass":
                    Console.WriteLine("If you pass, you will have to draw a card. Are you sure? (yes/no)");
                    if (Console.ReadLine()?.Trim().ToLower() == "yes")
                    {
                        Console.WriteLine("You decided to pass your turn. Drawing one card.");
                        var newCard = Card.DrawCard(_gameState.AvailableCardsInDeck); 

                        if (newCard != null)
                        {
                            _player.Hand.Add(newCard);
                            Console.WriteLine($"You drew a card: {newCard}");
                        }

                        IsTurnOver = true;
                    }

                    break;

                default:
                    Console.WriteLine("Invalid action. Please type 'play', 'draw', or 'pass'.");
                    break;
            }
        }
    }


    private void AIPlayerTurn()
{
    // First, check if there are any playable cards in the AI's hand.
    var playableCards = _player.Hand.Where(IsValidMove).ToList();

    if (!playableCards.Any())
    {
        // No valid cards to play, so the AI must draw a card.
        var drawnCard = Card.DrawCard(_gameState.AvailableCardsInDeck); 

        if (drawnCard != null)
        {
            _player.Hand.Add(drawnCard);
            Console.WriteLine($"{_player.Nickname} (AI) drew a card.");

            // Check if the drawn card is playable.
            if (IsValidMove(drawnCard))
            {
                Console.WriteLine($"{_player.Nickname} (AI) played {drawnCard}.");
                ExecuteMove(drawnCard, _gameState);
            }
            else
            {
                Console.WriteLine($"{_player.Nickname} (AI) couldn't play the drawn card.");
            }
        }
        else
        {
            Console.WriteLine("No cards left to draw. Skipping turn.");
        }

        IsTurnOver = true;
        return;
    }

    // Strategy: If there are multiple playable cards, determine the best one based on predefined rules.

    // Rule 1: Prioritize cards by their type and value.
    var highValueSpecialCards = playableCards.Where(card => card.IsSpecialType())
                                             .OrderByDescending(card => card.GetCardValue());
    var highValueNumberCards = playableCards.Where(card => !card.IsSpecialType())
                                            .OrderByDescending(card => card.GetCardValue());
   var currentColor = _gameState.CurrentTopCard!.Color; 
    
    // Rule 2: Change the color card in play if possible.
    var differentColorCard = playableCards.FirstOrDefault(card => card.Color != currentColor);

    Card? selectedCard = null;

    if (highValueSpecialCards.Any())
    {
        selectedCard = highValueSpecialCards.First(); 
    }
    else if (differentColorCard != null)
    {
        selectedCard = differentColorCard;
    }
    else if (highValueNumberCards.Any())
    {
        selectedCard = highValueNumberCards.First(); 
    }

    // If the AI has a strategic card to play, execute the move.
    if (selectedCard != null)
    {
        Console.WriteLine($"{_player.Nickname} (AI) played {selectedCard}.");
        ExecuteMove(selectedCard, _gameState);
    }
    else
    {
        // Fallback if no strategic move is available, play the first playable card.
        var fallbackCard = playableCards.First();
        Console.WriteLine($"{_player.Nickname} (AI) played {fallbackCard}.");
        ExecuteMove(fallbackCard, _gameState);
    }

    IsTurnOver = true; // The AI's turn is over after making a move.
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