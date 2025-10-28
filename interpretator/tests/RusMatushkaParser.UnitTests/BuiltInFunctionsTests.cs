namespace RusMatushkaParser.UnitTests;

public class BuiltInFunctionsTests
{
    [Theory]
    [MemberData(nameof(GetModulFunctionTestData))]
    public void Evaluate_ModulFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetModulFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "модуль(-5)", 5m },
            { "модуль(5)", 5m },
            { "модуль(0)", 0m },
            { "модуль(2 * -3)", 6m },
            { "модуль(-2.5)", 2.5m },
        };
    }

    [Theory]
    [MemberData(nameof(GetLeastFunctionTestData))]
    public void Evaluate_LeastFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetLeastFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "малое(1, 2)", 1m },
            { "малое(5, 3, 8, 1)", 1m },
            { "малое(-1, -2, -3)", -3m },
            { "малое(2.5, 1.5, 3.5)", 1.5m },
            { "малое(10, 10, 10)", 10m },
        };
    }

    [Theory]
    [MemberData(nameof(GetGreatestFunctionTestData))]
    public void Evaluate_GreatestFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetGreatestFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "великое(1, 2)", 2m },
            { "великое(5, 3, 8, 1)", 8m },
            { "великое(-1, -2, -3)", -1m },
            { "великое(2.5, 1.5, 3.5)", 3.5m },
            { "великое(10, 10, 10)", 10m },
        };
    }

    [Theory]
    [MemberData(nameof(GetRoundFunctionTestData))]
    public void Evaluate_RoundFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetRoundFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "округлить(3.7)", 4m },
            { "округлить(3.2)", 3m },
            { "округлить(3.5)", 4m },
            { "округлить(2.5 + 1.3)", 4m },
            { "округлить(-2.7)", -3m },
            { "округлить(-2.2)", -2m },
        };
    }

    [Theory]
    [MemberData(nameof(GetCeilingFunctionTestData))]
    public void Evaluate_CeilingFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetCeilingFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "потолок(3.2)", 4m },
            { "потолок(3.0)", 3m },
            { "потолок(4.1 - 1.5)", 3m },
            { "потолок(-2.7)", -2m },
            { "потолок(-2.2)", -2m },
        };
    }

    [Theory]
    [MemberData(nameof(GetFloorFunctionTestData))]
    public void Evaluate_FloorFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetFloorFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "пол(3.8)", 3m },
            { "пол(3.0)", 3m },
            { "пол(5.9 / 2)", 2m },
            { "пол(-2.7)", -3m },
            { "пол(-2.2)", -3m },
        };
    }

    [Theory]
    [MemberData(nameof(GetPowerFunctionTestData))]
    public void Evaluate_PowerFunction_ReturnsCorrectResult(string expression, decimal expected)
    {
        // Arrange & Act
        decimal actualResult = Parser.EvaluateExpression(expression);

        // Assert
        Assert.Equal(expected, actualResult);
    }

    public static TheoryData<string, decimal> GetPowerFunctionTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "степень(2, 3)", 8m },
            { "степень(5, 2)", 25m },
            { "степень(4, 0.5)", 2m },
            { "степень(2, -1)", 0.5m },
            { "степень(5, 2 + 1)", 125m },
            { "степень(степень(2, 2), 2)", 16m },
        };
    }
}