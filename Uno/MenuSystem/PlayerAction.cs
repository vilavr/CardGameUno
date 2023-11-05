namespace MenuSystem;

public class PlayerAction
{
    private readonly GameState _gameState;
    private readonly Player _player;

    private readonly string jsonplayersfilepath = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../")), "Resources/players_info.json");

    public PlayerAction(Player player, GameState gameState)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
    }

    public bool IsTurnOver { get; private set; }

    public string? TakeTurn()
    {
        string? saving = null;
        Console.WriteLine($"Current Top Card: {_gameState.CurrentTopCard}");
        Console.WriteLine($"It's {_player.Nickname}'s turn");

        if (_player.Type == EPlayerType.Human)
        {
            string? saveFlag = HumanPlayerTurn();
            if (saveFlag == "s")
            {
                saving = "s";
            }
            
        }
        else
            AIPlayerTurn();

        return saving;
    }

    private string? HumanPlayerTurn()
    {
        var validMoveMade = false;
        var cardDrawn = false;
        string? flagSave = null;
        var _gameSetup = new GameSetup();
        // foreach (var player in _gameState.Players)
        // {
        //     if (player.Id == 1) 
        //     {
        //         AddTestCardsToPlayer(player);
        //       
        //     }
        // }

        // Check if there are any playable cards in the player's hand
        var hasPlayableCards = _player.ShowHand().Any(card => IsValidMove(card));
        
        while (!validMoveMade && !IsTurnOver)
        {
            var hand = _player.ShowHand();
            Console.WriteLine("Your cards:");
            for (var i = 0; i < hand.Count; i++) Console.WriteLine($"{i + 1}: {hand[i]}");

            if (hasPlayableCards)
                Console.WriteLine(
                    "Actions: 'play' to play a card, 'draw' to draw from the deck, 'pass' to skip your turn, 'save' to save your current game and exit.");
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
                            ExecuteMove(selectedCard, _player,
                                _gameState, _gameSetup, _gameState.Players, jsonplayersfilepath);
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

                        var newCard = Card.DrawCard(_gameState.AvailableCardsInDeck, _gameState);

                        if (newCard != null)
                        {
                            _player.AddCardAndUpdateJson(newCard);
                            Console.WriteLine($"You drew a card: {newCard}");

                            if (IsValidMove(newCard)) // check if the new card can be played immediately
                            {
                                Console.WriteLine("The card you drew is playable. Do you want to play it? (yes/no)");
                                if (Console.ReadLine()?.Trim().ToLower() == "yes")
                                    ExecuteMove(newCard, _player,
                                        _gameState, _gameSetup, _gameState.Players, jsonplayersfilepath);
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
                        var newCard = Card.DrawCard(_gameState.AvailableCardsInDeck, _gameState);

                        if (newCard != null)
                        {
                            _player.AddCardAndUpdateJson(newCard);
                            Console.WriteLine($"You drew a card: {newCard}");
                        }

                        IsTurnOver = true;
                    }

                    break;
                
                case "save":
                    // New save game logic
                    try
                    {
                        GameSaver gameSaver = new GameSaver();
                        gameSaver.SaveGame(_gameState); // assuming _gameState is your GameState object
                        Console.WriteLine("Game has been saved successfully.");
                        IsTurnOver = true;
                        validMoveMade = true;
                        flagSave = "s";
                        // You might want to decide whether saving the game should end the current turn or not.
                        // If it should, then you might set IsTurnOver = true; here.
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while saving the game: {ex.Message}");
                        // Optionally, log the exception or handle it accordingly.
                    }
                    break;

                default:
                    Console.WriteLine("Invalid action. Please type 'play', 'draw', or 'pass'.");
                    break;
            }
        }

        return flagSave;
    }


    private void AIPlayerTurn()
    {
        // First, check if there are any playable cards in the AI's hand.
        var playableCards = _player.Hand.Where(IsValidMove).ToList();

        var _gameSetup = new GameSetup();
        if (!playableCards.Any())
        {
            // No valid cards to play, so the AI must draw a card.
            var drawnCard = Card.DrawCard(_gameState.AvailableCardsInDeck, _gameState);

            if (drawnCard != null)
            {
                // Add the drawn card to the player's hand FIRST before any further checks.
                _player.AddCardAndUpdateJson(drawnCard);
                Console.WriteLine($"{_player.Nickname} (AI) drew a card.");

                // Now check if the drawn card is playable.
                if (IsValidMove(drawnCard))
                {
                    Console.WriteLine($"{_player.Nickname} (AI) played {drawnCard}.");

                    // Since the card is now in the hand, we can execute the move.
                    ExecuteMove(drawnCard, _player,
                        _gameState, _gameSetup, _gameState.Players, jsonplayersfilepath);
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
            selectedCard = highValueSpecialCards.First();
        else if (differentColorCard != null)
            selectedCard = differentColorCard;
        else if (highValueNumberCards.Any()) selectedCard = highValueNumberCards.First();

        // If the AI has a strategic card to play, execute the move.
        if (selectedCard != null)
        {
            Console.WriteLine($"{_player.Nickname} (AI) played {selectedCard}.");
            ExecuteMove(selectedCard, _player,
                _gameState, _gameSetup, _gameState.Players, jsonplayersfilepath);
        }
        else
        {
            // Fallback if no strategic move is available, play the first playable card.
            var fallbackCard = playableCards.First();
            Console.WriteLine($"{_player.Nickname} (AI) played {fallbackCard}.");
            ExecuteMove(fallbackCard, _player,
                _gameState, _gameSetup, _gameState.Players, jsonplayersfilepath);
        }

        IsTurnOver = true; // The AI's turn is over after making a move.
    }


    private bool IsValidMove(Card card)
    {
        if (card.Color == CardColor.Wild) return true;

        if (_gameState.CurrentTopCard == null) return false;

        return card.Color == _gameState.CurrentTopCard.Color || card.Value == _gameState.CurrentTopCard.Value;
    }

    public Player GetNextPlayer()
    {
        // Assuming the _players list and CurrentPlayerTurn property exist in the GameState
        var currentIndex = _gameState.Players.FindIndex(p => p.Id == _gameState.CurrentPlayerTurn);
        var nextIndex =
            (currentIndex + 1) % _gameState.Players.Count; // This ensures we loop back to the start of the list.
        return _gameState.Players[nextIndex];
    }

    private void ExecuteMove(Card card, Player player, GameState gameState, GameSetup gameSetup, List<Player> _players,
        string jsonFilePath)
    {
        if (!player.Hand.Contains(card))
            throw new InvalidOperationException("Attempted to discard a card not present in hand.");

        player.Hand.Remove(card);
        player.UpdatePlayerHandInJson(jsonFilePath, card, false);
        gameState.AddCardToDiscard(card);

        // Check if the round is won
        // Inside your PlayerAction class, where you determine the round's winner.
        if (!player.Hand.Any()) // A player wins the round if they have no cards left.
        {
            Console.WriteLine($"{player.Nickname} wins the round!");

            // Calculate the total score based on the remaining players' hands.
            var totalScore = _players.Where(p => p != player).Sum(p => p.Hand.Sum(c => c.Score));
            player.Score += totalScore; // Winner gets the total score of all cards remaining in other players' hands.

            Console.WriteLine($"Total score accumulated: {totalScore}");
            Console.WriteLine($"{player.Nickname}'s new total score: {player.Score}");

            // Save the updated scores and proceed to the next round.
            gameSetup.SavePlayersToJson(_players); // Save the updated scores to JSON.
    
            _gameState.CurrentPlayerTurn = player.Id; // The winner starts the next round.
            return; // The round is over, so exit the method.
        }


        // Execute special card rules
        switch (card.Value)
        {
            case CardValue.Skip:
                gameSetup.AdvanceTurn(_players, gameState);
                gameSetup.AdvanceTurn(_players, gameState);
                gameState.SpecialCardEffectApplied = true; // Set the flag after applying the card's effect
                break;


            case CardValue.Reverse:
                gameSetup.ReversePlayerOrder(_players, ref gameState);
                break;

            case CardValue.DrawTwo:
                DrawCardsForNextPlayer(2, gameState);
                gameSetup.AdvanceTurn(_players, gameState);
                gameSetup.AdvanceTurn(_players, gameState);
                gameState.SpecialCardEffectApplied = true;
                break;

            case CardValue.Wild:
                gameState.CurrentTopCard!.Color = PromptForColor(player);
                break;

            case CardValue.WildDrawFour:
                DrawCardsForNextPlayer(4, gameState);
                gameState.CurrentTopCard!.Color = PromptForColor(player);
                gameSetup.AdvanceTurn(_players, gameState);
                gameSetup.AdvanceTurn(_players, gameState);
                gameState.SpecialCardEffectApplied = true;
                break;
        }
    }

    private void DrawCardsForNextPlayer(int numberOfCards, GameState gameState)
    {
        var nextPlayer = GetNextPlayer();
        for (var i = 0; i < numberOfCards; i++)
        {
            var newCard = Card.DrawCard(gameState.AvailableCardsInDeck, _gameState);
            if (newCard != null) nextPlayer.AddCardAndUpdateJson(newCard);
        }
    }

    private CardColor PromptForColor(Player player)
    {
        if (player.Type == EPlayerType.Human)
        {
            Console.WriteLine(
                $"{player.Nickname}, you played a Wild card! Please choose a color (red, blue, green, yellow):");
            var input = string.Empty;
            while (true)
            {
                input = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;
                switch (input)
                {
                    case "red":
                        return CardColor.Red;
                    case "blue":
                        return CardColor.Blue;
                    case "green":
                        return CardColor.Green;
                    case "yellow":
                        return CardColor.Yellow;
                    default:
                        Console.WriteLine("Invalid color. Please enter red, blue, green, or yellow:");
                        break;
                }
            }
        }

        // If the player is an AI
        var random = new Random();
        var choice = random.Next(4);
        return (CardColor)choice;
    }


    // public void AddTestCardsToPlayer(Player player)
    // {
    //     // Create the action cards
    //     Card wildCard = new Card(CardColor.Wild, CardValue.Wild);
    //     Card wildDrawFourCard = new Card(CardColor.Wild, CardValue.WildDrawFour);
    //     Card skipCard = new Card(CardColor.Blue, CardValue.Skip); // Change color as needed
    //     Card reverseCard = new Card(CardColor.Green, CardValue.Reverse); // Change color as needed
    //     Card drawTwoCard = new Card(CardColor.Red, CardValue.DrawTwo); // Change color as needed
    //
    //     // Add the cards to the player's hand
    //     // player.ReceiveCard(wildCard);
    //     player.AddCardAndUpdateJson(wildCard);
    //     // player.ReceiveCard(wildDrawFourCard);
    //     player.AddCardAndUpdateJson(wildDrawFourCard);
    //     // player.ReceiveCard(skipCard);
    //     player.AddCardAndUpdateJson(skipCard);
    //     // player.ReceiveCard(reverseCard);
    //     player.AddCardAndUpdateJson(reverseCard);
    //     // player.ReceiveCard(drawTwoCard);
    //     player.AddCardAndUpdateJson(drawTwoCard);
    // }
}