namespace RusMatushkaParser;

public static class BuiltInFunctions
{
    private static readonly Dictionary<string, Func<List<decimal>, decimal>> Functions = new()
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
    };

    public static bool CheckBuiltInFunctions(string name)
    {
        return Functions.ContainsKey(name);
    }

    public static decimal Invoke(string name, List<decimal> arguments)
    {
        if (!Functions.TryGetValue(name, out Func<List<decimal>, decimal>? function))
        {
            throw new ArgumentException($"Неизвестная функция: {name}");
        }

        return function(arguments);
    }

    private static decimal Abs(List<decimal> arguments)
    {
        if (arguments.Count == 0)
        {
            throw new ArgumentException($"Использование: модуль(<число>)");
        }

        return Math.Abs(arguments[0]);
    }

    private static decimal Least(List<decimal> arguments)
    {
        return arguments.Min();
    }

    private static decimal Greatest(List<decimal> arguments)
    {
        return arguments.Max();
    }

    private static decimal Round(List<decimal> arguments)
    {
        return Math.Round(arguments[0]);
    }

    private static decimal Ceiling(List<decimal> arguments)
    {
        return Math.Ceiling(arguments[0]);
    }

    private static decimal Floor(List<decimal> arguments)
    {
        return Math.Floor(arguments[0]);
    }

    private static decimal Power(List<decimal> arguments)
    {
        if (arguments.Count < 2)
        {
            throw new ArgumentException($"Использование: степень(<число>,<кол-во степеней>)");
        }

        return (decimal)Math.Pow((double)arguments[0], (double)arguments[1]);
    }
}