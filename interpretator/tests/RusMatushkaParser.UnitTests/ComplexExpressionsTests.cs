namespace RusMatushkaParser.UnitTests;

public class ComplexExpressionsTests
{
    [Theory]
    [MemberData(nameof(GetComplexExpressionsData))]
    public void Handle_Complex_Expressions(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetComplexExpressionsData()
    {
        return new TheoryData<string, decimal>
        {
            { "малое(великое(1, 2), великое(3, 4))", 2m },
            { "округлить(потолок(3.14) * пол(2.7))", 8m },
            { "модуль(малое(-5, -10)) + великое(2, 3)", 13m },
            { "степень(округлить(3.7), пол(2.2))", 16m },
            { "модуль(-5) + малое(2, 3) * 4", 13m },
            { "(2 + модуль(-3)) * (5 - округлить(2.7))", 10m },
            { "-2 ^ 3 + модуль(-5) * малое(1, 2, 3)", -3m },
            { "2 + 3 * модуль(-4) ^ 2", 50m },
            { "округлить(потолок(3.14) * пол(2.7) + степень(2, 3))", 16m },
        };
    }
}