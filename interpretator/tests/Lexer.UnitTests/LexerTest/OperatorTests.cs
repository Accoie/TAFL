namespace Lexer.UnitTests;

public class OperatorTests : LexerTest
{
    [Theory]
    [MemberData(nameof(ArithmeticOperatorsData))]
    public void Can_tokenize_arithmetic_operators(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> ArithmeticOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "+ - * / % =",
                new List<Token>
                {
                    new(TokenType.PlusSign),
                    new(TokenType.MinusSign),
                    new(TokenType.MultiplySign),
                    new(TokenType.DivideSign),
                    new(TokenType.ModuloSign),
                    new(TokenType.Assign),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(LogicalOperatorsData))]
    public void Can_tokenize_logical_operators(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> LogicalOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "|| @ !",
                new List<Token>
                {
                    new(TokenType.LogicalOr),
                    new(TokenType.LogicalAnd),
                    new(TokenType.LogicalNot),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(ComparisonOperatorsData))]
    public void Can_tokenize_comparison_operators(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> ComparisonOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "== != < > <= >=",
                new List<Token>
                {
                    new(TokenType.Equal),
                    new(TokenType.NotEqual),
                    new(TokenType.LessThan),
                    new(TokenType.GreaterThan),
                    new(TokenType.LessThanOrEqual),
                    new(TokenType.GreaterThanOrEqual),
                }
            },
        };
    }
}