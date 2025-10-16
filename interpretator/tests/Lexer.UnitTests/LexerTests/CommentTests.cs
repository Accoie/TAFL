namespace Lexer.UnitTests;

public class CommentTests
{
    [Theory]
    [MemberData(nameof(SingleLineCommentsData))]
    public void Can_handle_single_line_comments(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> SingleLineCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                """
                МОЛВИ("test") ## это комментарий
                МОЛВИ("end")
                """,
                new List<Token>
                {
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("test")),
                    new(TokenType.RParen),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("end")),
                    new(TokenType.RParen),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(MultiLineCommentsData))]
    public void Can_handle_multi_line_comments(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> MultiLineCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                """
                МОЛВИ("start") /# это
                многострочный
                комментарий #/ МОЛВИ("end")
                """,
                new List<Token>
                {
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("start")),
                    new(TokenType.RParen),
                    new(TokenType.Output),
                    new(TokenType.LParen),
                    new(TokenType.StringLiteral, new TokenValue("end")),
                    new(TokenType.RParen),
                }
            },
        };
    }
}