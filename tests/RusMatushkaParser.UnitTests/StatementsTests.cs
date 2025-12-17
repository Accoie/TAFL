using Execution;

using RusMatushkaInterpreter;

using RusMatushkaParser.Exceptions;

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
            // Разбор выражений с несколькими переменными
            { "ЧИСЛО x : ДРОБЬ = 5; ЧИСЛО y : ДРОБЬ = 2; ЧИСЛО z : ДРОБЬ = степень(x, y);", "z", new decimal[] { 25m } },
            { "ЧИСЛО x : ДРОБЬ = 25; ЧИСЛО y : ДРОБЬ = 5; ЧИСЛО z : ДРОБЬ = x * y;", "z", new decimal[] { 125m } },

            // Разбор инициализации переменной дробного типа
            { "ЧИСЛО x : ДРОБЬ = 25.5 * 2.0 / 2.0;", "x", new decimal[] { 25.5m } },
            { "ЧИСЛО x : ДРОБЬ = -4.7;", "x", new decimal[] { -4.7m } },

            // Разбор присвоения переменной значения
            { "ЧИСЛО y : ДРОБЬ; y = 555.8;", "y", new decimal[] { 555.8m } },

            // Разбор потока вывода с двумя переменными
            { "ЧИСЛО x : ДРОБЬ = 10.2; ЧИСЛО y : ДРОБЬ = 12.2;", "x, y", new decimal[] { 10.2m, 12.2m } },

            // Разбор объявления переменной без инициализации
            { "ЧИСЛО x : ДРОБЬ;", "x", new decimal[] { 0 } },

            // Разбор области видимости (shadowing переменных)
            { "ЧИСЛО x : ДРОБЬ = 25.5; НАЧАЛО ЧИСЛО x : ДРОБЬ = 10; МОЛВИ(x); ИСХОД", "x", new decimal[] { 10, 25.5m } },

            // Разбор присвоения переменной значения неинициализированной переменной
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
            // Разбор использования переменной, которая инициализирована в блоке выше
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
            // Разбор объявления переменной, которая уже объявлена
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
            // Разбор потока ввода с несуществующей переменной
            { "ВНЕМЛИ(x);" },

            // Разбор потока вывода с неинициализированной переменной
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
            // Разбор без типа возвращаемого значения
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ) НАЧАЛО ИСХОД",
                new decimal[0]
            },

            // Разбор изменения переменной, объявленной вне функции
            {
                "ЧИСЛО num:ДРОБЬ = 5; ФУНКЦИЯ plusFive(x: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x + num; ИСХОД МОЛВИ(plusFive(4));",
                new decimal[] { 9m }
            },

            // Разбор с параметрами и возвратом вычисленного значения
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД МОЛВИ(sum(3, 4));",
                new decimal[] { 7m }
            },

            // Разбор с выводом в консоль после return
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; МОЛВИ(55); ИСХОД МОЛВИ(sum(3, 4));",
                new decimal[] { 7m }
            },

            // Разбор рекурсии
            {
                "ФУНКЦИЯ sum(x: ДРОБЬ):ДРОБЬ НАЧАЛО ЕСЛИ (x <= 1) СТАЛОБЫТЬ НАЧАЛО ДАРОВАТЬ x; ИСХОД ДАРОВАТЬ sum(x-1); ИСХОД МОЛВИ(sum(7));",
                new decimal[] { 1m }
            },

            // Разбор условия if
            {
                "ЧИСЛО x : ДРОБЬ = 3; ЕСЛИ (x == 3) СТАЛОБЫТЬ НАЧАЛО МОЛВИ(x); ИСХОД",
                new decimal[] { 3m }
            },

            // Разбор условия if-else
            {
                "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ (x == 3) СТАЛОБЫТЬ НАЧАЛО x = 5; ИСХОД ИНО НАЧАЛО x = 3; ИСХОД МОЛВИ(x);",
                new decimal[] { 3m }
            },

            // Разбор цикла while
            {
                "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x <= 3) ТВОРИ НАЧАЛО x = x + 1; ИСХОД МОЛВИ(x);",
                new decimal[] { 4m }
            },

            // Разбор цикла while с break
            {
                "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(ИСТИНА) ТВОРИ НАЧАЛО ЕСЛИ (x == 5) СТАЛОБЫТЬ НАЧАЛО ВЫЙТИ; ИСХОД x = x + 1; ИСХОД МОЛВИ(x);",
                new decimal[] { 5m }
            },

            // Разбор цикла while с continue
            {
                "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 6) ТВОРИ НАЧАЛО x = x + 1; ЕСЛИ (x % 2 == 0) СТАЛОБЫТЬ НАЧАЛО ПРОДОЛЖИТЬ; ИСХОД МОЛВИ(x); ИСХОД",
                new decimal[] { 1m, 3m, 5m }
            },

            // Разбор цикла for
            {
                "ЧИСЛО x : ДРОБЬ = 0; ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО x = x + i; ИСХОД МОЛВИ(x);",
                new decimal[] { 15m }
            },

            // Разбор цикла for в обратном порядке
            {
                "ДЛЯ i ОТ 5 ДО 1 ТВОРИ НАЧАЛО МОЛВИ(i); ИСХОД",
                new decimal[] { 5m, 4m, 3m, 2m, 1m }
            },

            // Разбор цикла for с break
            {
                "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО ЕСЛИ (i == 3) СТАЛОБЫТЬ НАЧАЛО ВЫЙТИ; ИСХОД МОЛВИ(i); ИСХОД",
                new decimal[] { 1m, 2m }
            },

            // Разбор цикла for с continue
            {
                "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО ЕСЛИ (i == 3) СТАЛОБЫТЬ НАЧАЛО ПРОДОЛЖИТЬ; ИСХОД МОЛВИ(i); ИСХОД",
                new decimal[] { 1m, 2m, 4m, 5m }
            },

            // Разбор вложенного цикла for
            {
                "ЧИСЛО sum:ДРОБЬ = 0; ДЛЯ i ОТ 1 ДО 3 ТВОРИ НАЧАЛО ДЛЯ j ОТ 1 ДО 2 ТВОРИ НАЧАЛО sum = sum + i * j; ИСХОД ИСХОД МОЛВИ(sum);",
                new decimal[] { 18m }
            },

            // Разбор вызова функции с параметрами
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
            // Разбор цикла, где счетчик дробное значение
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
            // Разбор цикла while с break вне цикла
            { "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 3) ТВОРИ НАЧАЛО x = x + 1; ИСХОД ВЫЙТИ; МОЛВИ(x);" },

            // Разбор цикла while с continue вне цикла
            { "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 3) ТВОРИ НАЧАЛО x = x + 1; ИСХОД ПРОДОЛЖИТЬ;МОЛВИ(x);" },

            // Разбор цикла for с break вне цикла
            { "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО МОЛВИ(i); ИСХОД ВЫЙТИ;" },

            // Разбор цикла for с continue вне цикла
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
            // Разбор функции без параметров
            { "ФУНКЦИЯ sum():ДРОБЬ НАЧАЛО ИСХОД" },

            // Разбор функции с параметром, у которого не указан тип
            { "ФУНКЦИЯ sum(x):ДРОБЬ НАЧАЛО МОЛВИ(2); ИСХОД" },

            // Разбор вызова функции с количеством параметров меньше, чем в объявлении
            { "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД МОЛВИ(sum(4));" },

            // Разбор висячего else
            { "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ (x == 3) СТАЛОБЫТЬ ЕСЛИ(x == 4) СТАЛОБЫТЬ МОЛВИ(3); ИНО МОЛВИ(2);" },

            // Разбор цикла for с функцией, которая вызывает break
            { "ДЛЯ i ОТ ДО 5 ТВОРИ НАЧАЛО ФУНКЦИЯ sum(x: ДРОБЬ):ДРОБЬ break ДАРОВАТЬ 1; ИСХОД ИСХОД" },
        };
    }
}