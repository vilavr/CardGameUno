namespace MenuSystem;

public class Card
{
    public CardColor Color { get; }
    public CardValue Value { get; }

    public Card(CardColor color, CardValue value)
    {
        Color = color;
        Value = value;
    }

    public override string ToString()
    {
        // This will return a string in the format "Color Value", e.g., "Red Three"
        return $"{Color} {Value}";
    }
}
