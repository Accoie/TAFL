namespace RusMatushkaParser.UnitTests;

public class BuiltInFunctionsTests
{
    [Theory]
    [MemberData(nameof(GetBuiltInFunctionsData))]
    public void Can_Handle_Built_In_Functions(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetBuiltInFunctionsData()
    {
        return new TheoryData<string, decimal>
        {
            { "модуль(-5)", 5m },
            { "модуль(-2.5)", 2.5m },
            { "малое(5, 3, 8, 1)", 1m },
            { "малое(2.5, 1.5, 3.5)", 1.5m },
            { "малое(10, 10, -10)", -10m },
            { "великое(5, 3, 8, 1)", 8m },
            { "великое(-1, -2, -3)", -1m },
            { "округлить(3.7)", 4m },
            { "округлить(3.2)", 3m },
            { "округлить(3.5)", 4m },
            { "потолок(3.2)", 4m },
            { "потолок(3.0)", 3m },
            { "потолок(-2.7)", -2m },
            { "пол(3.8)", 3m },
            { "пол(3.0)", 3m },
            { "пол(-2.2)", -3m },
            { "степень(5, 2)", 25m },
            { "степень(4, 0.5)", 2m },
            { "степень(2, -1)", 0.5m },
        };
    }
}