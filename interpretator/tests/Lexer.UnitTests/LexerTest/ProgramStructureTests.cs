namespace Lexer.UnitTests;

public class ProgramStructureTests : LexerTest
{
    [Theory]
    [MemberData(nameof(BasicProgramCasesData))]
    public void Can_tokenize_basic_program_constructions(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> BasicProgramCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "НАЧАЛО СЛОВО словечко; ВНЕМЛИ(словечко); МОЛВИ(словечко) ИСХОД",
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.Word),
                    new(TokenType.Identifier, new TokenValue("словечко")),
                    new(TokenType.Semicolon),
                    new(TokenType.Input),
                    new(TokenType.LParen),
                    new(TokenType.Identifier, new TokenValue("словечко")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.Identifier, new TokenValue("словечко")),
                    new(TokenType.RParen),
                    new(TokenType.End),
                }
            },
            {
                "начало МОЛВИ(\"test\"); исход",
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("test")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.End),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(VariableDeclarationCasesData))]
    public void Can_tokenize_variable_declarations(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> VariableDeclarationCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "СЛОВО а; ЧИСЛО б : цес = 1,2,3;",
                new List<Token>
                {
                    new(TokenType.Word),
                    new(TokenType.Identifier, new TokenValue("а")),
                    new(TokenType.Semicolon),
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("б")),
                    new(TokenType.Colon),
                    new(TokenType.IntegerType),
                    new(TokenType.Assign),
                    new(TokenType.Integer, new TokenValue(1)),
                    new(TokenType.Comma),
                    new(TokenType.Integer, new TokenValue(2)),
                    new(TokenType.Comma),
                    new(TokenType.Integer, new TokenValue(3)),
                    new(TokenType.Semicolon),
                }
            },
            {
                "ЧИСЛО лета : цес = 25; ЧИСЛО пи : дробь = 3.14; СЛОВО имя = \"Владимир\"",
                new List<Token>
                {
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("лета")),
                    new(TokenType.Colon),
                    new(TokenType.IntegerType),
                    new(TokenType.Assign),
                    new(TokenType.Integer, new TokenValue(25)),
                    new(TokenType.Semicolon),
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("пи")),
                    new(TokenType.Colon),
                    new(TokenType.FloatType),
                    new(TokenType.Assign),
                    new(TokenType.Float, new TokenValue(3.14m)),
                    new(TokenType.Semicolon),
                    new(TokenType.Word),
                    new(TokenType.Identifier, new TokenValue("имя")),
                    new(TokenType.Assign),
                    new(TokenType.StringLiteral, new TokenValue("Владимир")),
                }
            },
            {
                "число а = 5;",
                new List<Token>
                {
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("а")),
                    new(TokenType.Assign),
                    new(TokenType.Integer, new TokenValue(5)),
                    new(TokenType.Semicolon),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(ConditionalCasesData))]
    public void Can_tokenize_conditional_constructions(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> ConditionalCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "ЕЖЕЛИ (лета >= 18) СТАЛОБЫТЬ МОЛВИ(\"Здорово, \", имя); ИНО МОЛВИ(\"Мир тебе, \", имя);",
                new List<Token>
                {
                    new(TokenType.If),
                    new(TokenType.LParen),
                    new(TokenType.Identifier, new TokenValue("лета")),
                    new(TokenType.GreaterThanOrEqual),
                    new(TokenType.Integer, new TokenValue(18)),
                    new(TokenType.RParen),
                    new(TokenType.Then),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("Здорово, ")),
                    new(TokenType.Comma),
                    new(TokenType.Identifier, new TokenValue("имя")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.Else),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("Мир тебе, ")),
                    new(TokenType.Comma),
                    new(TokenType.Identifier, new TokenValue("имя")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(LoopCasesData))]
    public void Can_tokenize_loops(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> LoopCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "ДЛЯ счет ОТ 1 ДО 10 ТВОРИ МОЛВИ(\"Счет: \", счет);",
                new List<Token>
                {
                    new(TokenType.For),
                    new(TokenType.Identifier, new TokenValue("счет")),
                    new(TokenType.From),
                    new(TokenType.Integer, new TokenValue(1)),
                    new(TokenType.To),
                    new(TokenType.Integer, new TokenValue(10)),
                    new(TokenType.Do),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("Счет: ")),
                    new(TokenType.Comma),
                    new(TokenType.Identifier, new TokenValue("счет")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                }
            },
            {
                "ПОКУДА (истина) ТВОРИ НАЧАЛО МОЛВИ(\"Бесконечный цикл\"); ВЫЙТИ; ИСХОД",
                new List<Token>
                {
                    new(TokenType.While),
                    new(TokenType.LParen),
                    new(TokenType.True),
                    new(TokenType.RParen),
                    new(TokenType.Do),
                    new(TokenType.Begin),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("Бесконечный цикл")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.Break),
                    new(TokenType.Semicolon),
                    new(TokenType.End),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(NestedBlockCasesData))]
    public void Can_tokenize_nested_blocks(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> NestedBlockCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                """
                НАЧАЛО
                    ЕЖЕЛИ (истина) СТАЛОБЫТЬ
                        НАЧАЛО
                            МОЛВИ("внутренний блок");
                        ИСХОД
                    ИНО
                        МОЛВИ("внешний блок");
                ИСХОД
                """,
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.If),
                    new(TokenType.LParen),
                    new(TokenType.True),
                    new(TokenType.RParen),
                    new(TokenType.Then),
                    new(TokenType.Begin),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("внутренний блок")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.End),
                    new(TokenType.Else),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("внешний блок")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.End),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(ComplexProgramCasesData))]
    public void Can_tokenize_complex_programs(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> ComplexProgramCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                """
                НАЧАЛО
                    ЧИСЛО а : цес = 10;
                    ЧИСЛО б : дробь = 2.5;
                    СЛОВО текст = "результат: ";
                    БУЛЕВО флаг = истина;
                    
                    ЕЖЕЛИ (флаг @ а > 5) СТАЛОБЫТЬ
                        МОЛВИ(текст, а + б * 2);
                ИСХОД
                """,
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("а")),
                    new(TokenType.Colon),
                    new(TokenType.IntegerType),
                    new(TokenType.Assign),
                    new(TokenType.Integer, new TokenValue(10)),
                    new(TokenType.Semicolon),
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("б")),
                    new(TokenType.Colon),
                    new(TokenType.FloatType),
                    new(TokenType.Assign),
                    new(TokenType.Float, new TokenValue(2.5m)),
                    new(TokenType.Semicolon),
                    new(TokenType.Word),
                    new(TokenType.Identifier, new TokenValue("текст")),
                    new(TokenType.Assign),
                    new(TokenType.StringLiteral, new TokenValue("результат: ")),
                    new(TokenType.Semicolon),
                    new(TokenType.BooleanType),
                    new(TokenType.Identifier, new TokenValue("флаг")),
                    new(TokenType.Assign),
                    new(TokenType.True),
                    new(TokenType.Semicolon),
                    new(TokenType.If),
                    new(TokenType.LParen),
                    new(TokenType.Identifier, new TokenValue("флаг")),
                    new(TokenType.LogicalAnd),
                    new(TokenType.Identifier, new TokenValue("а")),
                    new(TokenType.GreaterThan),
                    new(TokenType.Integer, new TokenValue(5)),
                    new(TokenType.RParen),
                    new(TokenType.Then),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.Identifier, new TokenValue("текст")),
                    new(TokenType.Comma),
                    new(TokenType.Identifier, new TokenValue("а")),
                    new(TokenType.PlusSign),
                    new(TokenType.Identifier, new TokenValue("б")),
                    new(TokenType.MultiplySign),
                    new(TokenType.Integer, new TokenValue(2)),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.End),
                }
            },
        };
    }
}