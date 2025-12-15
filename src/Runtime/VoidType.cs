namespace Runtime;

/// <summary>
/// Специальный тип, обозначающий отсутствие значения.
/// </summary>
public record struct VoidType
{
    public static readonly VoidType Value;

    public override string ToString()
    {
        return "<void>";
    }
}