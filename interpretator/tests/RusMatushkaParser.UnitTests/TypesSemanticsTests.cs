using Execution;

namespace RusMatushkaParser.UnitTests;

public class TypeSemanticsTests
{
    private readonly FakeEnvironment environment;
    private readonly Context context;

    public TypeSemanticsTests()
    {
        environment = new FakeEnvironment();
        context = new Context();
    }

    [Theory]
    [MemberData(nameof(GetPositiveCasesData))]
    public void Can_Parse_Type_Semantics_With_Positive_Cases(string expression, string[] expectedOutput)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expectedOutput, environment.Strings.ToArray());
    }

    public static TheoryData<string, string[]> GetPositiveCasesData()
    {
        return new TheoryData<string, string[]>
        {
            // Разбор конкатенации строк
            {
                "СТРОКА x = \"привет, \" + \"андрей\"; МОЛВИ(x);",
                new[] { "привет, андрей" }
            },

            // Разбор операций сравнения для СТРОКА
            {
                "ЕСЛИ(\"s\" == \"s\") СТАЛОБЫТЬ НАЧАЛО МОЛВИ(\"s\"); ИСХОД",
                new[] { "s" }
            },
            {
                "ЕСЛИ(\"s\" != \"d\") СТАЛОБЫТЬ НАЧАЛО МОЛВИ(\"s\"); ИСХОД",
                new[] { "s" }
            },

            // Разбор операций сравнения для БУЛЕВО
            {
                "ЧИСЛО x : ДРОБЬ = 3; ЕСЛИ((x < 0) == ЛОЖЬ) СТАЛОБЫТЬ НАЧАЛО МОЛВИ(ИСТИНА); ИСХОД",
                new[] { "ИСТИНА" }
            },
            {
                "ЕСЛИ(ИСТИНА != ЛОЖЬ) СТАЛОБЫТЬ НАЧАЛО МОЛВИ(ИСТИНА); ИСХОД",
                new[] { "ИСТИНА" }
            },

            // Разбор логических операций
            {
                "БУЛЕВО x = 3 > 0; БУЛЕВО y = x @ ЛОЖЬ; МОЛВИ(y);",
                new[] { "ЛОЖЬ" }
            },
            {
                "БУЛЕВО x = 3 > 0; БУЛЕВО y = x || ЛОЖЬ; МОЛВИ(y);",
                new[] { "ИСТИНА" }
            },
            {
                "БУЛЕВО x = 3 > 0; МОЛВИ(!x);",
                new[] { "ЛОЖЬ" }
            },

            // Разбор работы функции без возвращаемого значения
            {
                "ФУНКЦИЯ приветствие(имя: СТРОКА) НАЧАЛО МОЛВИ(\"Привет, \", имя); ИСХОД приветствие(\"андрей\");",
                new[] { "Привет, ", "андрей" }
            },
            {
                "ФУНКЦИЯ приветствие(имя: СТРОКА) НАЧАЛО МОЛВИ(\"Привет, \", имя); ДАРОВАТЬ; ИСХОД приветствие(\"андрей\");",
                new[] { "Привет, ", "андрей" }
            },

            // Разбор операций сравнения для СТРОКА
            { "МОЛВИ(\"css\" > \"ssx\");", new[] { "ЛОЖЬ" } },
            { "МОЛВИ(\"css\" < \"ssx\");", new[] { "ИСТИНА" } },
            { "МОЛВИ(\"css\" >= \"ssx\");", new[] { "ЛОЖЬ" } },
            { "МОЛВИ(\"css\" <= \"ssx\");", new[] { "ИСТИНА" } },
        };
    }

    [Theory]
    [MemberData(nameof(GetNegativeCasesData))]
    public void Parse_Type_Semantics_With_Negative_Cases(string expression, Type expectedException)
    {
        // Arrange
        string code = $"НАЧАЛО {expression} ИСХОД";
        Parser parser = CreateParser(code);

        // Act & Assert
        Assert.Throws(expectedException, parser.ParseProgram);
    }

    public static TheoryData<string, Type> GetNegativeCasesData()
    {
        return new TheoryData<string, Type>
        {
            // Разбор инструкции условия, где условие не 0 или 1
            { "ЧИСЛО x : ДРОБЬ = 2; ЕСЛИ(x) СТАЛОБЫТЬ НАЧАЛО МОЛВИ(x); ИСХОД", typeof(TypeErrorException) },

            // Разбор присвоения строковой переменной числа
            { "СТРОКА x; x = 5;", typeof(TypeErrorException) },

            // Разбор несоответствия типов
            { "СТРОКА x = \"пример\"; x = 4;", typeof(TypeErrorException) },
            { "СТРОКА x = \"пример\"; ЧИСЛО y : ДРОБЬ = 2; y = x;", typeof(TypeErrorException) },

            // Разбор запрета бинарной операции(кроме +) со строкой
            { "СТРОКА x = \"пример\"; МОЛВИ(x-x);", typeof(TypeErrorException) },
            { "СТРОКА x = \"пример\"; МОЛВИ(x*x);", typeof(TypeErrorException) },
            { "СТРОКА x = \"пример\"; МОЛВИ(x/x);", typeof(TypeErrorException) },
            { "СТРОКА x = \"пример\"; МОЛВИ(x%x);", typeof(TypeErrorException) },

            // Разбор запрета операций сравнения для БУЛЕВО
            { "ЕСЛИ(ИСТИНА > ЛОЖЬ) СТАЛОБЫТЬ НАЧАЛО ИСХОД", typeof(TypeErrorException) },
            { "ЕСЛИ(ИСТИНА < ЛОЖЬ) СТАЛОБЫТЬ НАЧАЛО ИСХОД", typeof(TypeErrorException) },
            { "ЕСЛИ(ИСТИНА >= ЛОЖЬ) СТАЛОБЫТЬ НАЧАЛО ИСХОД", typeof(TypeErrorException) },
            { "ЕСЛИ(ИСТИНА <= ЛОЖЬ) СТАЛОБЫТЬ НАЧАЛО ИСХОД", typeof(TypeErrorException) },

            // Разбор запрета использований логических операций для типов кроме БУЛЕВО
            { "БУЛЕВО y = 1 @ 0;", typeof(TypeErrorException) },
            { "БУЛЕВО x = 3 > 0; БУЛЕВО y = x || 0;", typeof(TypeErrorException) },
            { "БУЛЕВО y = !1;", typeof(TypeErrorException) },
            { "МОЛВИ(\"1\" @ \"2\");", typeof(TypeErrorException) },
            { "МОЛВИ(\"1\" || \"2\");", typeof(TypeErrorException) },
            { "МОЛВИ(!\"1\");", typeof(TypeErrorException) },

            // Разбор запрета функции без ДАРОВАТЬ, когда у нее есть какой-то тип
            { "ФУНКЦИЯ приветствие(имя: СТРОКА):СТРОКА НАЧАЛО МОЛВИ(\"Привет, \", имя); ИСХОД МОЛВИ(приветствие(\"андрей\"));", typeof(TypeErrorException) },

            // Разбор запрета использования функции, которая не возвращает результат, в качестве выражения
            { "ФУНКЦИЯ приветствие(имя: СТРОКА) НАЧАЛО МОЛВИ(\"Привет, \", имя); ИСХОД МОЛВИ(приветствие(\"андрей\"));", typeof(InvalidOperationException) },

            // Разбор возврата несовместимого типа
            { "ФУНКЦИЯ приветствие(имя: СТРОКА):СТРОКА НАЧАЛО МОЛВИ(\"Привет, \", имя); ДАРОВАТЬ 3; ИСХОД приветствие(\"привы\");", typeof(TypeErrorException) },

            // Разбор несовместимых аргументов в встроенных функциях
            { "МОЛВИ(малое(1.00, 2.00, \"бяк\"));", typeof(TypeErrorException) },

            // Разбор запрета использования типа кроме БУЛЕВО в цикле while
            { "ПОКУДА(\"ИСТИНА\") ТВОРИ НАЧАЛО ИСХОД", typeof(InvalidOperationException) },
            { "ПОКУДА(1) ТВОРИ НАЧАЛО ИСХОД", typeof(InvalidOperationException) },

            // Разбор запрета использования типа кроме ДРОБЬ в цикле for
            { "СТРОКА x = \"555\"; ДЛЯ i ОТ 1 ДО x ТВОРИ НАЧАЛО ИСХОД", typeof(InvalidOperationException) },
        };
    }

    private Parser CreateParser(string code)
    {
        return new Parser(context, environment, code);
    }
}
