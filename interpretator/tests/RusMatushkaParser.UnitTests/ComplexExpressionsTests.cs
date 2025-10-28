namespace RusMatushkaParser.UnitTests;

public class ComplexExpressionsTests
{
    [Theory]
    [MemberData(nameof(GetComplexFunctionExpressionsTestData))]
    public void Evaluate_ComplexFunctionExpressions_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetComplexFunctionExpressionsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "малое(великое(1, 2), великое(3, 4))", 2m },
            { "округлить(потолок(3.14) * пол(2.7))", 8m },
            { "модуль(малое(-5, -10)) + великое(2, 3)", 13m },
            { "степень(округлить(3.7), пол(2.2))", 16m },
        };
    }

    [Theory]
    [MemberData(nameof(GetFunctionsWithOperatorsTestData))]
    public void Evaluate_FunctionsWithOperators_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetFunctionsWithOperatorsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "модуль(-5) + малое(2, 3) * 4", 13m },
            { "(2 + модуль(-3)) * (5 - округлить(2.7))", 10m },
            { "-2 ^ 3 + модуль(-5) * малое(1, 2, 3)", -3m },
        };
    }

    [Theory]
    [MemberData(nameof(GetSingleNumberExpressionsTestData))]
    public void Evaluate_SingleNumberExpressions_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetSingleNumberExpressionsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "42", 42m },
        };
    }

    [Theory]
    [MemberData(nameof(GetFullMathExpressionsTestData))]
    public void Evaluate_FullMathExpressions_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetFullMathExpressionsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "2 + 3 * модуль(-4) ^ 2", 50m },
            { "округлить(потолок(3.14) * пол(2.7) + степень(2, 3))", 16m },
        };
    }
}