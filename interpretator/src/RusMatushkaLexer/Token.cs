public class Token
{
    public Token(TokenType type)
    {
        Type = type;
        Value = new TokenValue(string.Empty);
    }

    public Token(TokenType type, TokenValue value)
    {
        Type = type;
        Value = value;
    }

    public TokenType Type { get; }

    public TokenValue Value { get; }

    public override bool Equals(object? obj)
    {
        return obj is Token other && Type == other.Type && Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Value);
    }
}