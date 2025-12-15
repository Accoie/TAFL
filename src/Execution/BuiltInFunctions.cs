using System.Globalization;

using Ast.Statements;

using Runtime;

using ValueType = Runtime.ValueType;

namespace RusMatushkaParser;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public class BuiltInFunctions
{
    public BuiltInFunctions()
    {
        List<BuiltInFunction> functions =
        [
            new BuiltInFunction(
                "модуль",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    decimal number = arguments[0].AsDecimal();
                    return new Value(Math.Abs(number));
                }
            ),

            new BuiltInFunction(
                "малое",
                [new BuiltInFunctionParameter("числа", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    if (arguments.Count == 0)
                    {
                        throw new ArgumentException("Использование: малое(<число1>, <число2>, ...)");
                    }

                    decimal min = arguments[0].AsDecimal();
                    for (int i = 1; i < arguments.Count; i++)
                    {
                        decimal current = arguments[i].AsDecimal();
                        if (current < min)
                        {
                            min = current;
                        }
                    }

                    return new Value(min);
                }
            ),

            new BuiltInFunction(
                "великое",
                [new BuiltInFunctionParameter("числа", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    if (arguments.Count == 0)
                    {
                        throw new ArgumentException("Использование: великое(<число1>, <число2>, ...)");
                    }

                    decimal max = arguments[0].AsDecimal();
                    for (int i = 1; i < arguments.Count; i++)
                    {
                        decimal current = arguments[i].AsDecimal();
                        if (current > max)
                        {
                            max = current;
                        }
                    }

                    return new Value(max);
                }
            ),

            new BuiltInFunction(
                "округлить",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    decimal number = arguments[0].AsDecimal();
                    return new Value(Math.Round(number));
                }
            ),

            new BuiltInFunction(
                "потолок",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    decimal number = arguments[0].AsDecimal();
                    return new Value(Math.Ceiling(number));
                }
            ),

            new BuiltInFunction(
                "пол",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    decimal number = arguments[0].AsDecimal();
                    return new Value(Math.Floor(number));
                }
            ),

            new BuiltInFunction(
                "степень",
                [new BuiltInFunctionParameter("число", ValueType.Float), new BuiltInFunctionParameter("степень", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    decimal number = arguments[0].AsDecimal();
                    decimal power = arguments[1].AsDecimal();
                    return new Value((decimal)Math.Pow((double)number, (double)power));
                }
            ),

            new BuiltInFunction(
                "числовстроку",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.String,
                arguments =>
                {
                    decimal number = arguments[0].AsDecimal();

                    if (number % 1 == 0)
                    {
                        return new Value(((int)number).ToString());
                    }
                    else
                    {
                        return new Value(number.ToString("0.00", CultureInfo.InvariantCulture));
                    }
                }
            ),
        ];

        Functions = functions.ToDictionary(function => function.Name);
    }

    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public IReadOnlyDictionary<string, BuiltInFunction> Functions { get; }
}