using Execution;

namespace RusMatushkaParser.UnitTests;

public class LogicalAndComparisonTests
{
    private readonly FakeEnvironment environment;
    private readonly Context context;

    public LogicalAndComparisonTests()
    {
        environment = new FakeEnvironment();
        context = new Context();
    }

    [Theory]
    [MemberData(nameof(GetLogicalOperationsTestData))]
    public void Handle_Logical_Operations(string expression, decimal expected)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal([expected], environment.Results);
    }

    [Theory]
    [MemberData(nameof(GetComparisonOperationsTestData))]
    public void Handle_Comparison_Operations(string expression, decimal expected)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal([expected], environment.Results);
    }

    [Theory]
    [MemberData(nameof(GetComparisonWithVariablesTestData))]
    public void Handle_Comparison_With_Variables(string code, decimal expected)
    {
        // Arrange
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal([expected], environment.Results);
    }

    [Theory]
    [MemberData(nameof(GetInvalidLogicalOperationsTestData))]
    public void Handle_Invalid_Logical_Operations(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    public static TheoryData<string, decimal> GetLogicalOperationsTestData()
    {
        return new TheoryData<string, decimal>
        {
            // Логическое И
            { "1 @ 1", 1m },
            { "0 @ 1", 0m },

            // Логическое ИЛИ
            { "1 || 0", 1m },
            { "0 || 0", 0m },

            // Логическое НЕ
            { "!1", 0m },
            { "!(!1)", 1m },

            // Комбинация логических операторов
            { "1 @ 0 || 1", 1m },

            // Приоритет логических операторов (И имеет приоритет над ИЛИ)
            { "1 || 0 @ 1", 1m }, // 1 || (0 @ 1) = 1 || 0 = 1

            // Ассоциативность логических операторов (левая ассоциативность)
            { "1 @ 0 @ 1", 0m }, // (1 @ 0) @ 1 = 0 @ 1 = 0
        };
    }

    public static TheoryData<string, decimal> GetComparisonOperationsTestData()
    {
        return new TheoryData<string, decimal>
        {
            // Равенство чисел
            { "3.14 == 3.14", 1m },
            { "3.14 == 3.24", 0m },

            // Неравенство чисел
            { "5 != 3", 1m },
            { "3.5 != 3.5", 0m },

            // Сравнения чисел
            { "3.2 > 2.8", 1m },
            { "4 <= 4", 1m },
            { "5 >= 3", 1m },
            { "2.5 < 2.7", 1m },

            // Сравнения с выражениями
            { "10 / 2 > 3", 1m }, // 5 > 3 = 1
        };
    }

    public static TheoryData<string, decimal> GetComparisonWithVariablesTestData()
    {
        return new TheoryData<string, decimal>
        {
            { "НАЧАЛО ЧИСЛО x : ЦЕС = 0; ЧИСЛО y : ЦЕС = 2; МОЛВИ(x < y); ИСХОД", 1m },
        };
    }

    public static TheoryData<string> GetInvalidLogicalOperationsTestData()
    {
        return new TheoryData<string>
        {
            { "2 @ 1" }, // Ошибка при числе больше 1
        };
    }
}