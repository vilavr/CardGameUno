namespace MenuSystem;

public class Player
{
    public int Id { get; }
    public string Nickname { get; set; }
    public EPlayerType Type { get; set; }

    public Player(int id, string nickname, EPlayerType type)
    {
        Id = id;
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
        Type = type;
    }
    
}
