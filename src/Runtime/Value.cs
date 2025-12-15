using System.Globalization;

namespace Runtime;

public class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VoidType.Value);

    private readonly object value;

    public Value(string value)
    {
        this.value = value;
    }

    public Value(decimal value)
    {
        this.value = value;
    }

    public Value(bool value)
    {
        this.value = value;
    }

    private Value(VoidType value)
    {
        this.value = value;
    }

    /// <summary>
    /// Возвращает тип значения.
    /// </summary>
    public ValueType GetValueType()
    {
        return value switch
        {
            string => ValueType.String,
            decimal => ValueType.Float,
            bool => ValueType.Bool,
            VoidType => ValueType.Void,
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
        };
    }

    /// <summary>
    /// Возвращает значение как строку либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {value} is not a string"),
        };
    }

    /// <summary>
    /// Возвращает значение как дробное число либо бросает исключение.
    /// </summary>
    public decimal AsDecimal()
    {
        return value switch
        {
            decimal i => i,
            _ => throw new InvalidOperationException($"Value {value} is not an decimal"),
        };
    }

    /// <summary>
    /// Возвращает значение как логический тип данных либо бросает исключение.
    /// </summary>
    public bool AsBool()
    {
        return value switch
        {
            bool b => b,
            _ => throw new InvalidOperationException($"Value {value} is not an bool"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return value switch
        {
            string s => s,
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            bool b => b ? "ИСТИНА" : "ЛОЖЬ",
            VoidType v => "ПУСТОТА",
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
        };
    }

    /// <summary>
    /// Сравнивает на равенство два значения.
    /// </summary>
    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        if (GetValueType() != other.GetValueType())
        {
            return false;
        }

        return value switch
        {
            string s => other.AsString() == s,
            decimal d => Numbers.AreEqual(d, other.AsDecimal()),
            bool b => other.AsBool() == b,
            VoidType => true,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}