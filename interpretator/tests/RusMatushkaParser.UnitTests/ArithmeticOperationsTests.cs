namespace RusMatushkaParser.UnitTests;

public class ArithmeticOperationsTests
{
    [Theory]
    [MemberData(nameof(GetIntegerLiteralsTestData))]
    public void Evaluate_IntegerLiterals_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetIntegerLiteralsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "0", 0m },
            { "42", 42m },
            { "100", 100m },
            { "999", 999m },
        };
    }

    [Theory]
    [MemberData(nameof(GetRealLiteralsTestData))]
    public void Evaluate_RealLiterals_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetRealLiteralsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "3.14", 3.14m },
            { "0.5", 0.5m },
            { "2.0", 2.0m },
            { "0.25", 0.25m },
            { "123.456", 123.456m },
        };
    }

    [Theory]
    [MemberData(nameof(GetParenthesesExpressionsTestData))]
    public void Evaluate_ParenthesesExpressions_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetParenthesesExpressionsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "(5 + 3)", 8m },
            { "(2 * (3 + 4))", 14m },
            { "((1 + 2) * 3)", 9m },
        };
    }

    [Theory]
    [MemberData(nameof(GetModuloOperationsTestData))]
    public void Evaluate_ModuloOperations_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetModuloOperationsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "10 % 3", 1m },
            { "15 % 4 % 2", 1m },
        };
    }

    [Theory]
    [MemberData(nameof(GetDivisionByZeroTestData))]
    public void Evaluate_DivisionByZero_ThrowsException(string expression)
    {
        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => Parser.EvaluateExpression(expression));
    }

    public static TheoryData<string> GetDivisionByZeroTestData()
    {
        return new TheoryData<string>
        {
            { "10 / 0" },
            { "15 % 0" },
        };
    }

    [Theory]
    [MemberData(nameof(GetUnaryPlusTestData))]
    public void Evaluate_UnaryPlus_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetUnaryPlusTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "+5", 5m },
            { "+3.14", 3.14m },
        };
    }

    [Theory]
    [MemberData(nameof(GetUnaryMinusTestData))]
    public void Evaluate_UnaryMinus_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetUnaryMinusTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "-10", -10m },
            { "-2.5", -2.5m },
        };
    }

    [Theory]
    [MemberData(nameof(GetPowerOperationsTestData))]
    public void Evaluate_PowerOperations_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetPowerOperationsTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "2 ^ 3", 8m },
            { "5 ^ 2", 25m },
            { "2 ^ 3 ^ 2", 512m }, // Правая ассоциативность: 2^(3^2) = 2^9 = 512
        };
    }

    [Theory]
    [MemberData(nameof(GetOperatorPrecedenceTestData))]
    public void Evaluate_OperatorPrecedence_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetOperatorPrecedenceTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "2 + 3 * 4", 14m },        // 2 + (3 * 4) = 2 + 12 = 14
            { "10 - 8 / 2", 6m },        // 10 - (8 / 2) = 10 - 4 = 6
            { "2 * 3 ^ 2", 18m },        // 2 * (3 ^ 2) = 2 * 9 = 18
            { "(2 + 3) * 4", 20m },      // (2 + 3) * 4 = 5 * 4 = 20
            { "-2 ^ 3", -8m },           // -(2 ^ 3) = -8
            { "+3 ^ 2", 9m },             // +(3 ^ 2) = +9
        };
    }
}