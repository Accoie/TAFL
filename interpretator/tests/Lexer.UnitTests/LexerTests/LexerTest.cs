namespace Lexer.UnitTests;

public static class LexerTest
{
    public static List<Token> Tokenize(string code)
    {
        TextScanner scanner = new(code);
        Lexer lexer = new(scanner);
        List<Token> tokens = new List<Token>();

        Token token = lexer.ParseToken();
        while (token.Type != TokenType.EndOfFile)
        {
            tokens.Add(token);
            token = lexer.ParseToken();
        }

        return tokens;
    }

    public static void AssertTokensEqual(List<Token> expected, List<Token> actual)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Token expectedToken = expected[i];
            Token actualToken = actual[i];

            Assert.Equal(expectedToken.Type, actualToken.Type);
            Assert.Equal(expectedToken.Value, actualToken.Value);
        }
    }
}