using System.Globalization;

public class TokenValue
{
    private readonly object value;

    public TokenValue(string value)
    {
        this.value = value;
    }

    public TokenValue(decimal value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value switch
        {
            string s => s,
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotImplementedException(),
        };
    }

    public decimal ToDecimal()
    {
        return value switch
        {
            string s => decimal.Parse(s, CultureInfo.InvariantCulture),
            decimal d => d,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return value switch
            {
                string s => (string)other.value == s,
                decimal d => (decimal)other.value == d,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}