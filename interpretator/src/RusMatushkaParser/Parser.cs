using Execution;

namespace RusMatushkaParser;

/// <summary>
/// Выполняет синтаксический разбор.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly TokenStream tokens;

    public Parser(Context context, IEnvironment environment, string code)
    {
        this.context = context;
        this.environment = environment;
        tokens = new TokenStream(code);
    }

    /// <summary>
    /// Выполняет разбор выражения RusMatushka
    /// Правило:
    ///     program = "НАЧАЛО", { statement }, "ИСХОД".
    /// </summary>
    public void ParseProgram() // переделать чтобы блок ялвялся узлом ast.
    {
        Match(TokenType.Begin);
        context.PushScope(new Scope());
        do
        {
            ParseStatement();

            if (tokens.Peek().Type == TokenType.Semicolon)
            {
                Match(TokenType.Semicolon);
            }
        }
        while (tokens.Peek().Type != TokenType.EndOfFile);

        if (context.GetScopesCount() != 0)
        {
            throw new ArgumentException("Program's scope is not closed");
        }
    }

    /// <summary>
    /// Разбирает инструкцию верхнего уровня.
    /// Правило:
    ///     statement = variable_declaration
    ///        | assignment_statement
    ///        | output_statement
    ///        | input_statement
    ///        | block .
    ///
    ///     block = "НАЧАЛО", { statement }, "ИСХОД".
    /// </summary>
    private void ParseStatement()
    {
        TokenType token = tokens.Peek().Type;

        switch (token)
        {
            case TokenType.Identifier:
                ParseAssign();
                break;
            case TokenType.Number:
                Match(TokenType.Number);
                ParseNumberVariable();
                break;
            case TokenType.Begin:
                Match(TokenType.Begin);
                context.PushScope(new Scope());
                break;
            case TokenType.End:
                Match(TokenType.End);
                context.PopScope();
                break;
            case TokenType.Input:
                ParseInput();
                break;
            case TokenType.Output:
                ParseOutput();
                break;
        }

        if (token != TokenType.End && token != TokenType.Begin)
        {
            Match(TokenType.Semicolon);
        }
    }

    /// <summary>
    /// Разбирает инструкцию присвоения.
    /// assignment_statement = identifier, "=", expression, ";".
    /// </summary>
    private void ParseAssign()
    {
        decimal value = 0;
        string name = Match(TokenType.Identifier).Value!.ToString();

        if (tokens.Peek().Type == TokenType.Assign)
        {
            tokens.Advance();
            value = ParseExpression();
        }

        context.AssignVariable(name, value);
    }

    /// <summary>
    /// Разбирает инструкцию вывода.
    /// output_statement = "МОЛВИ", "(", argument_list, ")", ";".
    /// </summary>
    private void ParseOutput()
    {
        Match(TokenType.Output);
        Match(TokenType.LParen);
        WriteStringOrNumber();
        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            WriteStringOrNumber();
        }

        Match(TokenType.RParen);
        environment.WriteLine();
    }

    private void WriteStringOrNumber()
    {
        Token token = tokens.Peek();
        if (token.Type == TokenType.StringLiteral)
        {
            environment.WriteString(token.Value.ToString());
            tokens.Advance();
        }
        else
        {
            environment.WriteNumber(ParseExpression());
        }
    }

    /// <summary>
    /// Разбирает инструкцию ввода.
    /// input_statement = "ВНЕМЛИ", "(", identifier, ")", ";".
    /// </summary>
    private void ParseInput()
    {
        Match(TokenType.Input);
        Match(TokenType.LParen);

        decimal number = environment.ReadNumber();
        string name = tokens.Peek().Value!.ToString();
        context.AssignVariable(name, number);
        Match(TokenType.Identifier);
        Match(TokenType.RParen);
    }

    /// <summary>
    /// Разбирает объявление числовой переменной.
    /// variable_declaration = "ЧИСЛО", identifier, ":", type, [ "=", expression ], ";".
    /// type = "ДРОБЬ".
    /// </summary>
    private void ParseNumberVariable()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        decimal? value = null;
        switch (tokens.Peek().Type)
        {
            case TokenType.FloatType:
                tokens.Advance();
                if (tokens.Peek().Type == TokenType.Assign)
                {
                    tokens.Advance();
                    value = ParseExpression();
                }

                context.DefineVariable(name, value);
                break;
        }
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
    ///     primary_expression = identifier | number | function_call | "(", expression, ")" ;
    ///     number = float ;.
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
                string name = tokens.Peek().Value!.ToString();
                if (BuiltInFunctions.CheckBuiltInFunctions(name))
                {
                    return ParseFunctionCall();
                }
                else
                {
                    tokens.Advance();
                    return context.TryGetValue(name);
                }

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
    private Token Match(TokenType expected)
    {
        Token t = tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        tokens.Advance();

        return t;
    }
}