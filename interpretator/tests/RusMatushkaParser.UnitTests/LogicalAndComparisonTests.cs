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
    public void Handle_Logical_Operations(string expression, string expected)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Strings.Last());
    }

    public static TheoryData<string, string> GetLogicalOperationsTestData()
    {
        return new TheoryData<string, string>
        {
            // Логическое И
            { "ИСТИНА @ ИСТИНА", "ИСТИНА" },
            { "ЛОЖЬ @ ИСТИНА", "ЛОЖЬ" },

            // Логическое ИЛИ
            { "ИСТИНА || ЛОЖЬ", "ИСТИНА" },
            { "ЛОЖЬ || ЛОЖЬ", "ЛОЖЬ" },

            // Логическое НЕ
            { "!ИСТИНА", "ЛОЖЬ" },
            { "!(!ИСТИНА)", "ИСТИНА" },

            // Комбинация логических операторов
            { "ИСТИНА @ ЛОЖЬ || ИСТИНА", "ИСТИНА" },

            // Приоритет логических операторов
            { "ИСТИНА || ЛОЖЬ @ ИСТИНА", "ИСТИНА" },

            // Ассоциативность
            { "ИСТИНА @ ЛОЖЬ @ ИСТИНА", "ЛОЖЬ" },

            // Дополнительные примеры
            { "!(ИСТИНА || ЛОЖЬ)", "ЛОЖЬ" },
            { "!ИСТИНА @ !ЛОЖЬ", "ЛОЖЬ" },
            { "ИСТИНА || ЛОЖЬ @ ИСТИНА", "ИСТИНА" },
        };
    }

    [Theory]
    [MemberData(nameof(GetComparisonOperationsTestData))]
    public void Handle_Comparison_Operations(string expression, string expected)
    {
        // Arrange
        string code = $"НАЧАЛО МОЛВИ({expression}); ИСХОД";
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Strings.Last());
    }

    public static TheoryData<string, string> GetComparisonOperationsTestData()
    {
        return new TheoryData<string, string>
        {
            // Равенство чисел
            { "3.14 == 3.14", "ИСТИНА" },
            { "3.14 == 3.24", "ЛОЖЬ" },

            // Неравенство чисел
            { "5 != 3", "ИСТИНА" },
            { "3.5 != 3.5", "ЛОЖЬ" },

            // Сравнения чисел
            { "3.2 > 2.8", "ИСТИНА" },
            { "4 <= 4", "ИСТИНА" },
            { "5 >= 3", "ИСТИНА" },
            { "2.5 < 2.7", "ИСТИНА" },

            // Сравнения с выражениями
            { "10 / 2 > 3", "ИСТИНА" },
        };
    }

    [Theory]
    [MemberData(nameof(GetComparisonWithVariablesTestData))]
    public void Handle_Comparison_With_Variables(string code, string expected)
    {
        // Arrange
        Parser parser = new Parser(context, environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Strings.Last());
    }

    public static TheoryData<string, string> GetComparisonWithVariablesTestData()
    {
        return new TheoryData<string, string>
        {
            { "НАЧАЛО ЧИСЛО x : ДРОБЬ = 0; ЧИСЛО y : ДРОБЬ = 2; МОЛВИ(x < y); ИСХОД", "ИСТИНА" },
        };
    }
}
