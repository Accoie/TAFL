using Execution;
using RusMatushkaInterpreter;

using Semantics.Exceptions;

namespace RusMatushkaParser.UnitTests;

public class StatementsTests
{
    private readonly FakeEnvironment environment;
    private readonly Interpreter interpreter;

    public StatementsTests()
    {
        environment = new FakeEnvironment();
        interpreter = new Interpreter(environment);
    }

    [Theory]
    [MemberData(nameof(GetVariableStatementsPositiveData))]
    public void Can_Parse_Variable_Statements_With_Positive_Usage(string expression, string output, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} МОЛВИ({output}); ИСХОД";

        // Act
        interpreter.Execute(code);

        // Assert
        Assert.Equal(expected, environment.Numbers);
    }

    public static TheoryData<string, string, decimal[]> GetVariableStatementsPositiveData()
    {
        return new TheoryData<string, string, decimal[]>
        {
            { "ЧИСЛО x : ДРОБЬ = 5; ЧИСЛО y : ДРОБЬ = 2; ЧИСЛО z : ДРОБЬ = степень(x, y);", "z", new decimal[] { 25m } },
            { "ЧИСЛО x : ДРОБЬ = 25; ЧИСЛО y : ДРОБЬ = 5; ЧИСЛО z : ДРОБЬ = x * y;", "z", new decimal[] { 125m } },
            { "ЧИСЛО x : ДРОБЬ = 25.5 * 2.0 / 2.0;", "x", new decimal[] { 25.5m } },
            { "ЧИСЛО x : ДРОБЬ = -4.7;", "x", new decimal[] { -4.7m } },
            { "ЧИСЛО y : ДРОБЬ; y = 555.8;", "y", new decimal[] { 555.8m } },
            { "ЧИСЛО x : ДРОБЬ = 10.2; ЧИСЛО y : ДРОБЬ = 12.2;", "x, y", new decimal[] { 10.2m, 12.2m } },
            { "ЧИСЛО x : ДРОБЬ;", "x", new decimal[] { 0 } },
            { "ЧИСЛО x : ДРОБЬ = 25.5; НАЧАЛО ЧИСЛО x : ДРОБЬ = 10; МОЛВИ(x); ИСХОД", "x", new decimal[] { 10, 25.5m } },
            { "ЧИСЛО x : ДРОБЬ; ЧИСЛО y : ДРОБЬ = x;", "y", new decimal[] { 0 } },
        };
    }

    [Theory]
    [MemberData(nameof(GetScopesData))]
    public void Can_Parse_Scopes(string expression, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";

        // Act
        interpreter.Execute(code);

        // Assert
        Assert.Equal(expected, environment.Numbers);
    }

    public static TheoryData<string, decimal[]> GetScopesData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ЧИСЛО x : ДРОБЬ = 25.5; НАЧАЛО ЧИСЛО y : ДРОБЬ = x; МОЛВИ(y); ИСХОД",  new decimal[] { 25.5m } },
        };
    }

    [Theory]
    [MemberData(nameof(GetNegativeCasesData))]
    public void Parse_Float_Variables_With_Negative_Cases(string expression, string output)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} МОЛВИ({output}); ИСХОД";

        // Act & Assert
        Assert.Throws<DuplicateSymbolException>(() => interpreter.Execute(code));
    }

    public static TheoryData<string, string> GetNegativeCasesData()
    {
        return new TheoryData<string, string>
        {
            { "ЧИСЛО x : ДРОБЬ; ЧИСЛО x : ДРОБЬ;", "x" },
        };
    }

    [Fact]
    public void Parse_When_Block_Is_Not_Close()
    {
        // Arrange
        string code = "НАЧАЛО\"НАЧАЛО ЧИСЛО x : ДРОБЬ = 25.5;\" ИСХОД";

        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => interpreter.Execute(code));
    }

    [Theory]
    [MemberData(nameof(GetUseVariableTestData))]
    public void Can_Use_Variable_Which_Is_Not_Initialized(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";

        // Act & Assert
        Assert.Throws<UnknownSymbolException>(() => interpreter.Execute(code));
    }

    public static TheoryData<string> GetUseVariableTestData()
    {
        return new TheoryData<string>
        {
            { "ВНЕМЛИ(x);" },
            { "МОЛВИ(x);" },
        };
    }

    [Fact]
    public void Can_Initialize_Float_With_String_Literal()
    {
        // Arrange
        string code = "НАЧАЛО ЧИСЛО x : ДРОБЬ = \"dff\"; ИСХОД";

        // Act & Assert
        Assert.Throws<TypeMismatchException>(() => interpreter.Execute(code));
    }

    [Fact]
    public void Can_Write_With_String_And_Numbers()
    {
        // Arrange
        string code = "НАЧАЛО ЧИСЛО x : ДРОБЬ = 25.5; МОЛВИ(\"Числа: \", x, \", \", x); ИСХОД";
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        Interpreter consoleInterpreter = new Interpreter(new ConsoleEnvironment());
        consoleInterpreter.Execute(code);

        // Assert
        Assert.Equal("Числа: 25.5, 25.5\r\n", sw.ToString());
    }

    [Theory]
    [MemberData(nameof(GetStatementsPositiveData))]
    public void Parse_Statements_With_Positive_Cases(string expression, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";

        // Act
        interpreter.Execute(code);

        // Assert
        Assert.Equal(expected, environment.Numbers);
    }

    public static TheoryData<string, decimal[]> GetStatementsPositiveData()
    {
        return new TheoryData<string, decimal[]>
        {
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ) НАЧАЛО ИСХОД",
                new decimal[0]
            },
            {
                "ЧИСЛО num:ДРОБЬ = 5; ФУНКЦИЯ plusFive(x: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x + num; ИСХОД МОЛВИ(plusFive(4));",
                new decimal[] { 9m }
            },
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД МОЛВИ(sum(3, 4));",
                new decimal[] { 7m }
            },
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; МОЛВИ(55); ИСХОД МОЛВИ(sum(3, 4));",
                new decimal[] { 7m }
            },
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ):ДРОБЬ НАЧАЛО ЕСЛИ (x <= 1) СТАЛОБЫТЬ НАЧАЛО ДАРОВАТЬ x; ИСХОД ДАРОВАТЬ sum(x-1); ИСХОД МОЛВИ(sum(7));",
                new decimal[] { 1m }
            },
            {
                "ЧИСЛО x : ДРОБЬ = 3; ЕСЛИ (x == 3) СТАЛОБЫТЬ НАЧАЛО МОЛВИ(x); ИСХОД",
                new decimal[] { 3m }
            },
            {
                "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ (x == 3) СТАЛОБЫТЬ НАЧАЛО x = 5; ИСХОД ИНО НАЧАЛО x = 3; ИСХОД МОЛВИ(x);",
                new decimal[] { 3m }
            },
            {
                "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x <= 3) ТВОРИ НАЧАЛО x = x + 1; ИСХОД МОЛВИ(x);",
                new decimal[] { 4m }
            },
            {
                "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(ИСТИНА) ТВОРИ НАЧАЛО ЕСЛИ (x == 5) СТАЛОБЫТЬ НАЧАЛО ВЫЙТИ; ИСХОД x = x + 1; ИСХОД МОЛВИ(x);",
                new decimal[] { 5m }
            },
            {
                "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 6) ТВОРИ НАЧАЛО x = x + 1; ЕСЛИ (x % 2 == 0) СТАЛОБЫТЬ НАЧАЛО ПРОДОЛЖИТЬ; ИСХОД МОЛВИ(x); ИСХОД",
                new decimal[] { 1m, 3m, 5m }
            },
            {
                "ЧИСЛО x : ДРОБЬ = 0; ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО x = x + i; ИСХОД МОЛВИ(x);",
                new decimal[] { 15m }
            },
            {
                "ДЛЯ i ОТ 5 ДО 1 ТВОРИ НАЧАЛО МОЛВИ(i); ИСХОД",
                new decimal[] { 5m, 4m, 3m, 2m, 1m }
            },
            {
                "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО ЕСЛИ (i == 3) СТАЛОБЫТЬ НАЧАЛО ВЫЙТИ; ИСХОД МОЛВИ(i); ИСХОД",
                new decimal[] { 1m, 2m }
            },
            {
                "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО ЕСЛИ (i == 3) СТАЛОБЫТЬ НАЧАЛО ПРОДОЛЖИТЬ; ИСХОД МОЛВИ(i); ИСХОД",
                new decimal[] { 1m, 2m, 4m, 5m }
            },
            {
                "ЧИСЛО sum:ДРОБЬ = 0; ДЛЯ i ОТ 1 ДО 3 ТВОРИ НАЧАЛО ДЛЯ j ОТ 1 ДО 2 ТВОРИ НАЧАЛО sum = sum + i * j; ИСХОД ИСХОД МОЛВИ(sum);",
                new decimal[] { 18m }
            },
            {
                "ЧИСЛО x:ДРОБЬ = 0; ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД МОЛВИ(sum(2,3));",
                new decimal[] { 5 }
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetStatementsNegativeData))]
    public void Parse_Statements_With_Negative_Cases(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => interpreter.Execute(code));
    }

    public static TheoryData<string> GetStatementsNegativeData()
    {
        return new TheoryData<string>
        {
            { "ЧИСЛО x : ДРОБЬ; ДЛЯ i ОТ 1.1 ДО 5 ТВОРИ НАЧАЛО x = i; ИСХОД МОЛВИ(x);" },
        };
    }

    [Theory]
    [MemberData(nameof(GetStatementsNegativeSemanticsData))]
    public void Parse_Statements_With_Negative_Semantics(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";

        // Act & Assert
        Assert.Throws<InvalidExpressionException>(() => interpreter.Execute(code));
    }

    public static TheoryData<string> GetStatementsNegativeSemanticsData()
    {
        return new TheoryData<string>
        {
            { "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 3) ТВОРИ НАЧАЛО x = x + 1; ИСХОД ВЫЙТИ; МОЛВИ(x);" },
            { "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 3) ТВОРИ НАЧАЛО x = x + 1; ИСХОД ПРОДОЛЖИТЬ;МОЛВИ(x);" },
            { "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО МОЛВИ(i); ИСХОД ВЫЙТИ;" },
            { "ЧИСЛО x : ДРОБЬ; ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО x = i; ИСХОД ПРОДОЛЖИТЬ;" },
        };
    }

    [Theory]
    [MemberData(nameof(GetStatementsIncorrectSyntax))]
    public void Parse_Statements_With_IncorrectSyntax(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => interpreter.Execute(code));
    }

    public static TheoryData<string> GetStatementsIncorrectSyntax()
    {
        return new TheoryData<string>
        {
            { "ФУНКЦИЯ sum():ДРОБЬ НАЧАЛО ИСХОД" },
            { "ФУНКЦИЯ sum(x):ДРОБЬ НАЧАЛО МОЛВИ(2); ИСХОД" },
            { "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД МОЛВИ(sum(4));" },
            { "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ (x == 3) СТАЛОБЫТЬ ЕСЛИ(x == 4) СТАЛОБЫТЬ МОЛВИ(3); ИНО МОЛВИ(2);" },
            { "ДЛЯ i ОТ ДО 5 ТВОРИ НАЧАЛО ФУНКЦИЯ sum(x: ДРОБЬ):ДРОБЬ break ДАРОВАТЬ 1; ИСХОД ИСХОД" },
        };
    }
}