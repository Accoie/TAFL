namespace Lexer.UnitTests;

public class StructureTests : LexerTest
{
    [Theory]
    [MemberData(nameof(BracketCasesData))]
    public void Can_tokenize_brackets(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> BracketCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "{}()[]",
                new List<Token>
                {
                    new(TokenType.LBrace),
                    new(TokenType.RBrace),
                    new(TokenType.LParen),
                    new(TokenType.RParen),
                    new(TokenType.LBracket),
                    new(TokenType.RBracket),
                }
            },
            {
                "{([])}",
                new List<Token>
                {
                    new(TokenType.LBrace),
                    new(TokenType.LParen),
                    new(TokenType.LBracket),
                    new(TokenType.RBracket),
                    new(TokenType.RParen),
                    new(TokenType.RBrace),
                }
            },
            {
                "МОЛВИ{1, 2, 3}",
                new List<Token>
                {
                    new(TokenType.Output),
                    new(TokenType.LBrace),
                    new(TokenType.Integer, new TokenValue(1)),
                    new(TokenType.Comma),
                    new(TokenType.Integer, new TokenValue(2)),
                    new(TokenType.Comma),
                    new(TokenType.Integer, new TokenValue(3)),
                    new(TokenType.RBrace),
                }
            },
            {
                "ЧИСЛО массив[10]",
                new List<Token>
                {
                    new(TokenType.Number),
                    new(TokenType.Identifier, new TokenValue("массив")),
                    new(TokenType.LBracket),
                    new(TokenType.Integer, new TokenValue(10)),
                    new(TokenType.RBracket),
                }
            },
            {
                "ЕЖЕЛИ (условие) { МОЛВИ(\"test\"); }",
                new List<Token>
                {
                    new(TokenType.If),
                    new(TokenType.LParen),
                    new(TokenType.Identifier, new TokenValue("условие")),
                    new(TokenType.RParen),
                    new(TokenType.LBrace),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("test")),
                    new(TokenType.RParen),
                    new(TokenType.Semicolon),
                    new(TokenType.RBrace),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(PunctuationData))]
    public void Can_tokenize_punctuation(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> PunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "; , :",
                new List<Token>
                {
                    new(TokenType.Semicolon),
                    new(TokenType.Comma),
                    new(TokenType.Colon),
                }
            },
        };
    }
}