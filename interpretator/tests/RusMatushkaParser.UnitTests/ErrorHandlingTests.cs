using Execution;

namespace RusMatushkaParser.UnitTests;

public class ErrorHandlingTests
{
    private readonly FakeEnvironment environment;
    private readonly Context context;

    public ErrorHandlingTests()
    {
        environment = new FakeEnvironment();
        context = new Context();
    }

    [Theory]
    [MemberData(nameof(GetFalseExpressions))]
    public void Handle_False_Expressions(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetFalseExpressions()
    {
        return new TheoryData<string>
        {
            { "(2 + 3" },
            { "2 +" },
            { ".25" },
            { "3.14.15" },
            { "2 + 3)" },
            { "малое(1, 2,)" },
            { "малое(1 2)" },
        };
    }

    [Theory]
    [MemberData(nameof(GetFalseFunctionsExpressions))]
    public void Handle_FalseFunctions_Expressions(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetFalseFunctionsExpressions()
    {
        return new TheoryData<string>
        {
            { "модуль()" },
        };
    }
}