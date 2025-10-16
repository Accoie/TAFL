namespace Lexer.UnitTests;

public class KeywordTests
{
    [Theory]
    [MemberData(nameof(KeywordCasesData))]
    public void Can_tokenize_keywords(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> KeywordCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "НАЧАЛО СЛОВО ВНЕМЛИ МОЛВИ ИСХОД",
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.Word),
                    new(TokenType.Input),
                    new(TokenType.Output),
                    new(TokenType.End),
                }
            },
            {
                "ЕЖЕЛИ СТАЛОБЫТЬ ИНО",
                new List<Token>
                {
                    new(TokenType.If),
                    new(TokenType.Then),
                    new(TokenType.Else),
                }
            },
            {
                "ДЛЯ ОТ ДО ТВОРИ ПОКУДА ВЫЙТИ",
                new List<Token>
                {
                    new(TokenType.For),
                    new(TokenType.From),
                    new(TokenType.To),
                    new(TokenType.Do),
                    new(TokenType.While),
                    new(TokenType.Break),
                }
            },
            {
                "ЧИСЛО цес дробь БУЛЕВО",
                new List<Token>
                {
                    new(TokenType.Number),
                    new(TokenType.IntegerType),
                    new(TokenType.FloatType),
                    new(TokenType.BooleanType),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(CaseInsensitiveKeywordsData))]
    public void Can_tokenize_case_insensitive_keywords(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> CaseInsensitiveKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "начало МОЛВИ исход",
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.Output),
                    new(TokenType.End),
                }
            },
            {
                "число слово булево",
                new List<Token>
                {
                    new(TokenType.Number),
                    new(TokenType.Word),
                    new(TokenType.BooleanType),
                }
            },
        };
    }
}