namespace Lexer;

public static class LexicalStats
{
    public static string CollectFromFile(string path)
    {
        FileScanner fileScanner = new FileScanner(path);
        Lexer lexer = new Lexer(fileScanner);

        Dictionary<string, int> stats = new Dictionary<string, int>
        {
            { "keywords", 0 },
            { "identifier", 0 },
            { "number literals", 0 },
            { "string literals", 0 },
            { "operators", 0 },
            { "other lexemes", 0 },
        };

        Token token = lexer.ParseToken();
        while (token.Type != TokenType.EndOfFile)
        {
            if (token.Type != TokenType.EndOfFile)
            {
                CategorizeToken(token, stats);
            }

            token = lexer.ParseToken();
        }

        return FormatStats(stats);
    }

    private static void CategorizeToken(Token token, Dictionary<string, int> stats)
    {
        const string Keywords = "keywords";
        const string Identifier = "identifier";
        const string NumberLiterals = "number literals";
        const string StringLiterals = "string literals";
        const string Operators = "operators";
        const string OtherLexemes = "other lexemes";

        switch (token.Type)
        {
            case TokenType.Begin:
            case TokenType.End:
            case TokenType.Number:
            case TokenType.IntegerType:
            case TokenType.FloatType:
            case TokenType.Word:
            case TokenType.BooleanType:
            case TokenType.If:
            case TokenType.Then:
            case TokenType.Else:
            case TokenType.For:
            case TokenType.From:
            case TokenType.To:
            case TokenType.Do:
            case TokenType.While:
            case TokenType.Output:
            case TokenType.Input:
            case TokenType.LogicalNot:
            case TokenType.Break:
            case TokenType.Continue:
            case TokenType.Return:
                stats[Keywords]++;
                break;

            case TokenType.Identifier:
                stats[Identifier]++;
                break;

            case TokenType.Integer:
            case TokenType.Float:
                stats[NumberLiterals]++;
                break;

            case TokenType.StringLiteral:
                stats[StringLiterals]++;
                break;

            case TokenType.PlusSign:
            case TokenType.MinusSign:
            case TokenType.MultiplySign:
            case TokenType.DivideSign:
            case TokenType.ModuloSign:
            case TokenType.Assign:
            case TokenType.Equal:
            case TokenType.NotEqual:
            case TokenType.LessThan:
            case TokenType.LessThanOrEqual:
            case TokenType.GreaterThan:
            case TokenType.GreaterThanOrEqual:
            case TokenType.LogicalAnd:
            case TokenType.LogicalOr:
                stats[Operators]++;
                break;

            case TokenType.True:
            case TokenType.False:
            case TokenType.LBrace:
            case TokenType.RBrace:
            case TokenType.LParen:
            case TokenType.RParen:
            case TokenType.LBracket:
            case TokenType.RBracket:
            case TokenType.Semicolon:
            case TokenType.Comma:
            case TokenType.Colon:
            case TokenType.EndOfFile:
            case TokenType.Error:
                stats[OtherLexemes]++;
                break;
        }
    }

    private static string FormatStats(Dictionary<string, int> stats)
    {
        return string.Join("\n", stats.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }
}