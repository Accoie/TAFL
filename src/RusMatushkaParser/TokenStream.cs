using RusMatushkaLexer;

namespace RusMatushkaParser;

public class TokenStream
{
    private readonly Lexer lexer;
    private Token nextToken;

    public TokenStream(string text)
    {
        TextScanner scanner = new TextScanner(text);
        lexer = new Lexer(scanner);
        nextToken = lexer.ParseToken();
    }

    public Token Peek()
    {
        return nextToken;
    }

    public void Advance()
    {
        nextToken = lexer.ParseToken();
    }
}
