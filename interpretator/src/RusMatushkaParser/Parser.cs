namespace RusMatushkaParser;

/// <summary>
/// Выполняет синтаксический разбор.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;

    private Parser(string text)
    {
        tokens = new TokenStream(text);
    }

    public static decimal EvaluateExpression(string code)
    {
        Parser p = new(code);

        return p.ParseExpression();
    }

    /// <summary>
    /// Разбирает список выражений, разделенных запятыми.
    /// Правила:
    ///     argument_list = expression, { ",", expression } ;.
    /// </summary>
    private List<decimal> ParseExpressionList()
    {
        List<decimal> values =
        [
            ParseExpression(),
        ];
        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            values.Add(ParseExpression());
        }

        return values;
    }

    /// <summary>
    /// Разбирает выражение, содержащее операции сложения и вычитания.
    /// Правила:
    ///     expression = term_expression, { additive_operator, term_expression } ;
    ///     additive_operator = "+" | "-" ;.
    /// </summary>
    private decimal ParseExpression()
    {
        decimal value = ParseTermExpression();
        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    tokens.Advance();
                    value += ParseTermExpression();
                    break;
                case TokenType.MinusSign:
                    tokens.Advance();
                    value -= ParseTermExpression();
                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    /// Разбирает выражение, содержащее операции умножения, деления и остатка от деления.
    /// Правила:
    ///     term_expression = factor_expression, { multiplicative_operator, factor_expression } ;
    ///     multiplicative_operator = "*" | "/" | "%" ;.
    /// </summary>
    private decimal ParseTermExpression()
    {
        decimal value = ParseFactorExpression();
        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    tokens.Advance();
                    value *= ParseFactorExpression();
                    break;
                case TokenType.DivideSign:
                    tokens.Advance();
                    value /= ParseFactorExpression();
                    break;
                case TokenType.ModuloSign:
                    tokens.Advance();
                    value %= ParseFactorExpression();
                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    /// Разбирает выражение, которое может содержать унарные операторы.
    /// Правила:
    ///     factor_expression = [ unary_operator ], exponentiation_expression ;.
    ///     unary_operator = "+" | "-" ;.
    /// </summary>
    private decimal ParseFactorExpression()
    {
        switch (tokens.Peek().Type)
        {
            case TokenType.PlusSign:
                tokens.Advance();
                return ParseExponentiationExpression();

            case TokenType.MinusSign:
                tokens.Advance();
                return -ParseExponentiationExpression();

            default:
                return ParseExponentiationExpression();
        }
    }

    /// <summary>
    /// Разбирает выражение возведения в степень с правой ассоциативностью.
    /// Правила:
    ///     exponentiation_expression = primary_expression, [ "^", exponentiation_expression ] ;.
    /// </summary>
    private decimal ParseExponentiationExpression()
    {
        decimal value = ParsePrimaryExpression();
        if (tokens.Peek().Type == TokenType.ExponentiationSign)
        {
            tokens.Advance();
            value = (decimal)Math.Pow((double)value, (double)ParseExponentiationExpression());
        }

        return value;
    }

    /// <summary>
    /// Разбирает вызов функции.
    /// Правила:
    ///     function_call = function_name, "(", [ argument_list ], ")" ;
    ///     function_name = "модуль" | "малое" | "великое" | "округлить" | "потолок" | "пол" | "степень" ;
    ///     argument_list = expression, { ",", expression } ;.
    /// </summary>
    private decimal ParseFunctionCall()
    {
        string name = tokens.Peek().Value!.ToString();

        Match(TokenType.Identifier);
        Match(TokenType.LParen);

        List<decimal> arguments = ParseExpressionList();

        Match(TokenType.RParen);

        return BuiltInFunctions.Invoke(name, arguments);
    }

    /// <summary>
    /// Разбирает первичное выражение: число, идентификатор или выражение в скобках.
    /// Правила:
    ///     primary_expression = number | function_call | "(", expression, ")" ;
    ///     number = integer | float ;.
    /// </summary>
    private decimal ParsePrimaryExpression()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.Integer:
            case TokenType.Float:
                tokens.Advance();
                return t.Value!.ToDecimal();
            case TokenType.Identifier:
                return ParseFunctionCall();
            case TokenType.LParen:
                {
                    tokens.Advance();
                    decimal value = ParseExpression();
                    Match(TokenType.RParen);
                    return value;
                }

            default:
                throw new UnexpectedLexemeException(TokenType.Integer, t);
        }
    }

    /// <summary>
    /// Проверяет соответствие текущего токена ожидаемому типу и продвигает поток токенов.
    /// </summary>
    private void Match(TokenType expected)
    {
        Token t = tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        tokens.Advance();
    }
}