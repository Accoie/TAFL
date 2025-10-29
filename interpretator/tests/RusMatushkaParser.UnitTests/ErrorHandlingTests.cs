namespace RusMatushkaParser.UnitTests;

public class ErrorHandlingTests
{
    [Theory]
    [MemberData(nameof(GetBracketErrorTestData))]
    public void Evaluate_BracketErrors_ThrowsException(string expression)
    {
        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => Parser.EvaluateExpression(expression));
    }

    public static TheoryData<string> GetBracketErrorTestData()
    {
        return new TheoryData<string>
        {
            { "(2 + 3" },
        };
    }

    [Theory]
    [MemberData(nameof(GetBracketInEndTestData))]
    public void Evaluate_BracketInEnd_WillNotThrow(string expression)
    {
        // Act & Assert
        Exception? exception = Record.Exception(() => Parser.EvaluateExpression(expression));
        Assert.Null(exception);
    }

    public static TheoryData<string> GetBracketInEndTestData()
    {
        return new TheoryData<string>
        {
            { "2 + 3)" },
        };
    }

    [Theory]
    [MemberData(nameof(GetIncompleteExpressionTestData))]
    public void Evaluate_IncompleteExpressions_ThrowsException(string expression)
    {
        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => Parser.EvaluateExpression(expression));
    }

    public static TheoryData<string> GetIncompleteExpressionTestData()
    {
        return new TheoryData<string>
        {
            { "2 +" },
        };
    }

    [Theory]
    [MemberData(nameof(GetFunctionArgumentErrorTestData))]
    public void Evaluate_FunctionArgumentErrors_ThrowsException(string expression)
    {
        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => Parser.EvaluateExpression(expression));
    }

    public static TheoryData<string> GetFunctionArgumentErrorTestData()
    {
        return new TheoryData<string>
        {
            { "модуль()" },
            { "малое(1, 2,)" },
            { "малое(1 2)" },
        };
    }

    [Theory]
    [MemberData(nameof(GetInvalidNumberFormatTestData))]
    public void Evaluate_InvalidNumberFormat_ThrowsException(string expression)
    {
        // Act & Assert
        Exception? exception = Record.Exception(() => Parser.EvaluateExpression(expression));
        Assert.Null(exception);
    }

    public static TheoryData<string> GetInvalidNumberFormatTestData()
    {
        return new TheoryData<string>
        {
            { "3.14.15" },
        };
    }
}