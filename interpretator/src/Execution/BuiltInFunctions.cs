namespace RusMatushkaParser;

public static class BuiltInFunctions
{
    private static readonly Dictionary<string, Func<List<decimal>, object>> Functions = new()
    {
        {
            "модуль", Abs
        },
        {
            "малое", Least
        },
        {
            "великое", Greatest
        },
        {
            "округлить", Round
        },
        {
            "потолок", Ceiling
        },
        {
            "пол", Floor
        },
        {
            "степень", Power
        },
        {
            "числовстроку", NumberToString
        },
    };

    public static bool CheckBuiltInFunctions(string name)
    {
        return Functions.ContainsKey(name);
    }

    public static object Invoke(string name, List<decimal> arguments)
    {
        if (!Functions.TryGetValue(name, out Func<List<decimal>, object>? function))
        {
            throw new ArgumentException($"Неизвестная функция: {name}");
        }

        return function(arguments);
    }

    private static object Abs(List<decimal> arguments)
    {
        if (arguments.Count == 0)
        {
            throw new ArgumentException($"Использование: модуль(<число>)");
        }

        return Math.Abs(arguments[0]);
    }

    private static object Least(List<decimal> arguments)
    {
        return arguments.Min();
    }

    private static object Greatest(List<decimal> arguments)
    {
        return arguments.Max();
    }

    private static object Round(List<decimal> arguments)
    {
        return Math.Round(arguments[0]);
    }

    private static object Ceiling(List<decimal> arguments)
    {
        return Math.Ceiling(arguments[0]);
    }

    private static object Floor(List<decimal> arguments)
    {
        return Math.Floor(arguments[0]);
    }

    private static object Power(List<decimal> arguments)
    {
        if (arguments.Count < 2)
        {
            throw new ArgumentException($"Использование: степень(<число>,<кол-во степеней>)");
        }

        return (decimal)Math.Pow((double)arguments[0], (double)arguments[1]);
    }

    private static object NumberToString(List<decimal> arguments)
    {
        if (arguments.Count == 0)
        {
            throw new ArgumentException($"Использование: числовстроку(<число>)");
        }

        decimal number = arguments[0];

        if (number % 1 == 0)
        {
            return number.ToString("0");
        }
        else
        {
            return number.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}