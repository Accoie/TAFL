namespace RusMatushkaParser.UnitTests;

public class ErrorHandlingTests
{
    [Theory]
    [MemberData(nameof(GetFalseExpressions))]
    public void Handle_False_Expressions(string expression)
    {
        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => Parser.EvaluateExpression(expression));
    }

    public static TheoryData<string> GetFalseExpressions()
    {
        return new TheoryData<string>
        {
            { "модуль()" },
            { "малое(1, 2,)" },
            { "малое(1 2)" },
            { "(2 + 3" },
            { "2 +" },
            { ".25" },
        };
    }

    [Theory]
    [MemberData(nameof(GetPositiveExpressions))]
    public void Handle_Positive_Expressions(string expression)
    {
        // Act & Assert
        Parser.EvaluateExpression(expression);
    }

    public static TheoryData<string> GetPositiveExpressions()
    {
        return new TheoryData<string>
        {
            { "3.14.15" },
            { "2 + 3)" },
        };
    }
}