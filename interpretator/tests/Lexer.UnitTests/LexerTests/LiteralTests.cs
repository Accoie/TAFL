namespace Lexer.UnitTests;

public class LiteralTests
{
    [Theory]
    [MemberData(nameof(IntegerLiteralsData))]
    public void Can_tokenize_integer_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> IntegerLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "0 100 999 25",
                new List<Token>
                {
                    new(TokenType.Integer, new TokenValue(0)),
                    new(TokenType.Integer, new TokenValue(100)),
                    new(TokenType.Integer, new TokenValue(999)),
                    new(TokenType.Integer, new TokenValue(25)),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(FloatLiteralsData))]
    public void Can_tokenize_float_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> FloatLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "3.14 0.5 2.0",
                new List<Token>
                {
                    new(TokenType.Float, new TokenValue(3.14m)),
                    new(TokenType.Float, new TokenValue(0.5m)),
                    new(TokenType.Float, new TokenValue(2.0m)),
                }
            },
            {
                "3.14.14 0.5.0 2.0.3",
                new List<Token>
                {
                    new(TokenType.Float, new TokenValue(3.14m)),
                    new(TokenType.Error, new TokenValue(".")),
                    new(TokenType.Integer, new TokenValue(14)),
                    new(TokenType.Float, new TokenValue(0.5m)),
                    new(TokenType.Error, new TokenValue(".")),
                    new(TokenType.Integer, new TokenValue(0)),
                    new(TokenType.Float, new TokenValue(2.0m)),
                    new(TokenType.Error, new TokenValue(".")),
                    new(TokenType.Integer, new TokenValue(3)),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(StringLiteralsData))]
    public void Can_tokenize_string_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> StringLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "\"Владимир\" \"Здорово, \" \"Мир тебе, \"",
                new List<Token>
                {
                    new(TokenType.StringLiteral, new TokenValue("Владимир")),
                    new(TokenType.StringLiteral, new TokenValue("Здорово, ")),
                    new(TokenType.StringLiteral, new TokenValue("Мир тебе, ")),
                }
            },
            {
                "\"путь\\\\файл\" \"строка1\\нстрока2\"",
                new List<Token>
                {
                    new(TokenType.StringLiteral, new TokenValue("путь\\файл")),
                    new(TokenType.StringLiteral, new TokenValue("строка1\nстрока2")),
                }

            },
            {
                "\"путь\\\\файл\" \"строка1\\острока2\"",
                new List<Token>
                {
                    new(TokenType.StringLiteral, new TokenValue("путь\\файл")),
                    new(TokenType.StringLiteral, new TokenValue("строка1\tстрока2")),
                }

            },
            {
                "\"путь\\\\файл\" \"строка1\\кстрока2\"",
                new List<Token>
                {
                    new(TokenType.StringLiteral, new TokenValue("путь\\файл")),
                    new(TokenType.StringLiteral, new TokenValue("строка1\rстрока2")),
                }

            },
        };
    }

    [Theory]
    [MemberData(nameof(BooleanLiteralsData))]
    public void Can_tokenize_boolean_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> BooleanLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "ИСТИНА ЛОЖЬ",
                new List<Token>
                {
                    new(TokenType.True),
                    new(TokenType.False),
                }
            },
        };
    }
}