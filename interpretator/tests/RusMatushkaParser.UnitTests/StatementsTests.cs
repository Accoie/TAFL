using Execution;

namespace RusMatushkaParser.UnitTests;

public class StatementsTests
{
    private readonly FakeEnvironment environment;
    private readonly Context context;

    public StatementsTests()
    {
        environment = new FakeEnvironment();
        context = new Context();
    }

    [Theory]
    [MemberData(nameof(GetStatementsPositiveData))]
    public void Can_Parse_Statements_With_Positive_Usage(string expression, string output, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} МОЛВИ({output}); ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Results);
    }

    public static TheoryData<string, string, decimal[]> GetStatementsPositiveData()
    {
        return new TheoryData<string, string, decimal[]>
        {
            { "ЧИСЛО x : ДРОБЬ = 5; ЧИСЛО y : ДРОБЬ = 2; ЧИСЛО z : ДРОБЬ = степень(x, y);", "z", new decimal[] { 25m } }, // Разбор выражений с несколькими переменными
            { "ЧИСЛО x : ДРОБЬ = 25; ЧИСЛО y : ДРОБЬ = 5; ЧИСЛО z : ДРОБЬ = x * y;", "z", new decimal[] { 125m } }, // Разбор выражений с несколькими переменными
            { "ЧИСЛО x : ДРОБЬ = 25.5 * 2.0 / 2.0;", "x", new decimal[] { 25.5m } }, // Разбор инициализации переменной дробного типа
            { "ЧИСЛО x : ДРОБЬ = -4.7;", "x", new decimal[] { -4.7m } }, // Разбор инициализации переменной дробного типа
            { "ЧИСЛО y : ДРОБЬ; y = 555.8;", "y", new decimal[] { 555.8m } }, // Разбор присвоения переменной значения
            { "ЧИСЛО x : ДРОБЬ; ВНЕМЛИ(x);", "x", new decimal[] { 10.0m } }, // Разбор потока ввода c неинициализированной переменной
            { "ЧИСЛО x : ДРОБЬ = 10.2; ЧИСЛО y : ДРОБЬ = 12.2;", "x, y", new decimal[] { 10.2m, 12.2m } }, // Разбор потока вывода с двумя переменными
        };
    }

    [Theory]
    [MemberData(nameof(GetScopesData))]
    public void Can_Parse_Scopes(string expression, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Results);
    }

    public static TheoryData<string, decimal[]> GetScopesData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ЧИСЛО x : ДРОБЬ = 25.5; НАЧАЛО ЧИСЛО y : ДРОБЬ = x; МОЛВИ(y); ИСХОД",  new decimal[] { 25.5m } }, // Разбор использования переменной, которая инициализирована в блоке выше
        };
    }

    [Theory]
    [MemberData(nameof(GetNegativeCasesData))]
    public void Parse_Float_Variables_With_Negative_Cases(string expression, string output)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} МОЛВИ({output}); ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    public static TheoryData<string, string> GetNegativeCasesData()
    {
        return new TheoryData<string, string>
        {
            { "ЧИСЛО x : ДРОБЬ = 25.5; НАЧАЛО ЧИСЛО x : ДРОБЬ; ИСХОД", "x" }, // Разбор объявления переменной, которая уже объявлена в блоке выше
            { "ЧИСЛО x : ДРОБЬ;", "x" }, // Разбор потока вывода с неинициализированной переменной
            { "ЧИСЛО x : ДРОБЬ; ЧИСЛО x : ДРОБЬ;", "x" }, // Разбор потока вывода
            { "ЧИСЛО x : ДРОБЬ; ЧИСЛО y : ДРОБЬ = x;", "y" }, // Разбор потока вывода
        };
    }

    [Fact]
    public void Parse_When_Block_Is_Not_Close()
    {
        // Arrange
        string code = "НАЧАЛО\"НАЧАЛО ЧИСЛО x : ДРОБЬ = 25.5;\" ИСХОД";  // Разбор случая, когда нет ключевого слова 'ИСХОД'
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Theory]
    [MemberData(nameof(GetUseVariableTestData))]
    public void Can_Use_Variable_Which_Is_Not_Initialized(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws<VariableNotFoundException>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetUseVariableTestData()
    {
        return new TheoryData<string>
        {
            { "НАЧАЛО ЧИСЛО x : ДРОБЬ = 25.5; ИСХОД МОЛВИ(x);" }, // Разбор прекращения действия переменной за областью видимости
            { "ВНЕМЛИ(x);" }, // Разбор потока ввода с несуществующей переменной
            { "МОЛВИ(x);" }, // Разбор потока вывода с неинициализированной переменной
        };
    }

    [Fact]
    public void Can_Initialize_Float_With_String_Literal()
    {
        // Arrange
        string code = "НАЧАЛО ЧИСЛО x : ДРОБЬ = \"dff\"; ИСХОД"; // Разбор инициализации переменной дробного типа строкой
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Can_Write_With_String_And_Numbers()
    {
        string code = "НАЧАЛО ЧИСЛО x : ДРОБЬ = 25.5; МОЛВИ(\"Числа: \", x, \", \", x); ИСХОД"; // Разбор потока вывода со строкой в начале и множеством чисел
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        Parser parser = new Parser(context, new ConsoleEnvironment(), code);
        parser.ParseProgram();

        Assert.Equal("Числа: 25.5, 25.5\r\n", sw.ToString());
    }

    [Theory]
    [MemberData(nameof(GetFunctionDeclarationPositiveData))]
    public void Can_Parse_Function_Declarations_With_Positive_Usage(string functionCode, string callCode, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {functionCode} {callCode} ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Results);
    }

    public static TheoryData<string, string, decimal[]> GetFunctionDeclarationPositiveData()
    {
        return new TheoryData<string, string, decimal[]>
        {
            { "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД", "МОЛВИ(sum(3, 4));", new decimal[] { 7m } }, // Разбор с параметрами и возвратом вычисленного значения
            { "ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; МОЛВИ(55); ИСХОД", "МОЛВИ(sum(3, 4));", new decimal[] { 7m } }, // Разбор с параметрами и возвратом вычисленного значения
        };
    }

    [Theory]
    [MemberData(nameof(GetFunctionDeclarationNegativeData))]
    public void Parse_Function_Declarations_With_Negative_Cases(string functionCode)
    {
        // Arrange
        string code = $"НАЧАЛО {functionCode} ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetFunctionDeclarationNegativeData()
    {
        return new TheoryData<string>
        {
            { "ФУНКЦИЯ sum(x: ДРОБЬ) НАЧАЛО ИСХОД" }, // Разбор без типа возвращаемого значения
            { "ФУНКЦИЯ sum(x):ДРОБЬ НАЧАЛО МОЛВИ(2); ИСХОД" }, // Разбор с параметром, у которого не указан тип
            { "ФУНКЦИЯ sum():ДРОБЬ НАЧАЛО ИСХОД" }, // Разбор без параметров (это должно быть ошибкой по условию)
            { "ЧИСЛО x:ДРОБЬ = 0; ФУНКЦИЯ sum(x: ДРОБЬ, y: ДРОБЬ):ДРОБЬ НАЧАЛО ДАРОВАТЬ x+y; ИСХОД" }, // Разбор с переменной, которая объявлена до функции
        };
    }

    [Theory]
    [MemberData(nameof(GetIfElsePositiveData))]
    public void Can_Parse_IfElse_Statements_With_Positive_Usage(string expression, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Results);
    }

    public static TheoryData<string, decimal[]> GetIfElsePositiveData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ЧИСЛО x : ДРОБЬ = 3; ЕСЛИ (x == 3) СТАЛОБЫТЬ МОЛВИ(x);", new decimal[] { 3m } }, // Разбор условия
            { "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ (x == 3) СТАЛОБЫТЬ x = 5; ИНО x = 3; МОЛВИ(x);", new decimal[] { 3m } }, // Разбор условия с else
        };
    }

    [Theory]
    [MemberData(nameof(GetIfElseNegativeData))]
    public void Parse_IfElse_Statements_With_Negative_Cases(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetIfElseNegativeData()
    {
        return new TheoryData<string>
        {
            { "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ(x) СТАЛОБЫТЬ МОЛВИ(x);" }, // Разбор инструкции условия, где условие не 0 или 1
        };
    }

    [Theory]
    [MemberData(nameof(GetWhileLoopPositiveData))]
    public void Can_Parse_While_Loops_With_Positive_Usage(string expression, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Results);
    }

    public static TheoryData<string, decimal[]> GetWhileLoopPositiveData()
    {
        return new TheoryData<string, decimal[]>
    {
        {
            "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x <= 3) ТВОРИ x = x + 1; МОЛВИ(x);", new decimal[] { 4m }
        }, // Разбор цикла
        {
            "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(1) ТВОРИ НАЧАЛО ЕСЛИ (x == 5) СТАЛОБЫТЬ ВЫЙТИ; x = x + 1; ИСХОД МОЛВИ(x);",
            new decimal[] { 5m }
        }, // Разбор break
        {
            "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 6) ТВОРИ НАЧАЛО x = x + 1; ЕСЛИ (x % 2 == 0) СТАЛОБЫТЬ ПРОДОЛЖИТЬ;  МОЛВИ(x); ИСХОД",
            new decimal[] { 1m, 3m, 5m }
        }, // Разбор continue
    };
    }

    [Theory]
    [MemberData(nameof(GetWhileLoopNegativeData))]
    public void Parse_While_Loops_With_Negative_Cases(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetWhileLoopNegativeData()
    {
        return new TheoryData<string>
        {
            { "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 3) ТВОРИ x = x + 1; ВЫЙТИ; МОЛВИ(x);" }, // Разбор цикла с break вне цикла
            { "ЧИСЛО x:ДРОБЬ = 0; ПОКУДА(x < 3) ТВОРИ x = x + 1; ПРОДОЛЖИТЬ; МОЛВИ(x);" }, // Разбор цикла с continue вне цикла
        };
    }

    [Theory]
    [MemberData(nameof(GetForLoopPositiveData))]
    public void Can_Parse_For_Loops_With_Positive_Usage(string expression, decimal[] expected)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expected, environment.Results);
    }

    public static TheoryData<string, decimal[]> GetForLoopPositiveData()
    {
        return new TheoryData<string, decimal[]>
    {
        {
            "ЧИСЛО x : ДРОБЬ; ДЛЯ i ОТ 1 ДО 5 ТВОРИ x = i; МОЛВИ(x);",
            new decimal[] { 5m }
        }, // Разбор цикла
        {
            "ДЛЯ i ОТ 5 ДО 1 ТВОРИ МОЛВИ(i);",
            new decimal[] { 5m, 4m, 3m, 2m, 1m }
        }, // Разбор цикла в обратном порядке
        {
            "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО ЕСЛИ (i == 3) СТАЛОБЫТЬ ВЫЙТИ; МОЛВИ(i); ИСХОД",
            new decimal[] { 1m, 2m }
        }, // Разбор цикла с break
        {
            "ДЛЯ i ОТ 1 ДО 5 ТВОРИ НАЧАЛО ЕСЛИ (i == 3) СТАЛОБЫТЬ ПРОДОЛЖИТЬ; МОЛВИ(i); ИСХОД",
            new decimal[] { 1m, 2m, 4m, 5m }
        }, // Разбор цикла с continue
    };
    }

    [Theory]
    [MemberData(nameof(GetForLoopNegativeData))]
    public void Parse_For_Loops_With_Negative_Cases(string expression)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    public static TheoryData<string> GetForLoopNegativeData()
    {
        return new TheoryData<string>
        {
            { "ЧИСЛО x : ДРОБЬ; ДЛЯ i ОТ 1.1 ДО 5 ТВОРИ x = i; МОЛВИ(x);" }, // Разбор цикла, где счетчик дробное значение
            { "ДЛЯ i ОТ 1 ДО 5 ТВОРИ МОЛВИ(i); ВЫЙТИ;" }, // Разбор цикла с break вне цикла
            { "ЧИСЛО x : ДРОБЬ; ДЛЯ i ОТ 1 ДО 5 ТВОРИ x = i; ПРОДОЛЖИТЬ;" }, // Разбор цикла с continue вне цикла
        };
    }

    private Parser CreateParser(string code)
    {
        return new Parser(context, environment, code);
    }
}