using Execution;

namespace RusMatushkaParser.UnitTests;

public class ArithmeticOperationsTests
{
    private readonly FakeEnvironment environment;
    private readonly Context context;

    public ArithmeticOperationsTests()
    {
        environment = new FakeEnvironment();
        context = new Context();
    }

    [Theory]
    [MemberData(nameof(GetDivisionByZeroTestData))]
    public void Handle_Divide_By_Zero(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => parser.ParseProgram());
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
    [MemberData(nameof(GetArithmeticOperations))]
    public void Handle_Arithmetic_Operations(string expression, decimal expected)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal([expected], environment.Numbers);
    }

    public static TheoryData<string, decimal> GetArithmeticOperations()
    {
        return new TheoryData<string, decimal>
        {
            { "999", 999m },
            { "123.456", 123.456m },
            { "10 % 3", 1m },
            { "15 % 4 % 2", 1m },
            { "+5", 5m },
            { "+3.14", 3.14m },
            { "-10", -10m },
            { "-2.5", -2.5m },
            { "2 * +3", 6m },
            { "2 * -3", -6m },
            { "2 ^ 3 ^ 2", 512m },
            { "-2 + 5 + 5", 8m },
            { "4 * 2 / 4 * 0", 0m },
            { "4 / 2 / 4", 0.5m },
            { "4 % 4 % 4", 0m },
            { "степень(2, 5)", 32m },
            { "2 - 2 - 2", -2m },
            { "2 ^ 3 ^ 2", 512m },
            { "2 + 3 * 4", 14m },
            { "10 - 8 / 2", 6m },
            { "2 * 3 ^ 2", 18m },
            { "(2 + 3) * 4", 20m },
            { "-2 ^ 3", -8m },
        };
    }
}