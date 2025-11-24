using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Execution;

namespace RusMatushkaParser;

/// <summary>
/// Выполняет синтаксический разбор и строит AST.
/// </summary>
public class Parser
{
    private readonly Context context;
    private readonly TokenStream tokens;
    private readonly AstEvaluator evaluator;

    public Parser(Context context, IEnvironment environment, string code)
    {
        this.context = context;
        evaluator = new AstEvaluator(context, environment);
        tokens = new TokenStream(code);
    }

    /// <summary>
    /// Выполняет разбор выражения RusMatushka
    /// Правило: program = "НАЧАЛО", { statement }, "ИСХОД".
    /// </summary>
    public void ParseProgram()
    {
        evaluator.Visit(ParseBlock());

        if (context.GetScopesCount() != 0)
        {
            throw new ArgumentException("Program's scope is not closed");
        }
    }

    /// <summary>
    /// Разбирает инструкцию верхнего уровня.
    /// </summary>
    private AstNode ParseStatement()
    {
        TokenType token = tokens.Peek().Type;

        return token switch
        {
            TokenType.Identifier => ParseAssignmentOrExpression(),
            TokenType.Number => ParseNumberVariableDeclaration(),
            TokenType.Begin => ParseBlock(),
            TokenType.Input => ParseInput(),
            TokenType.Output => ParseOutput(),
            TokenType.Function => ParseFunctionDeclaration(),
            TokenType.If => ParseIfStatement(),
            TokenType.Return => ParseReturnStatement(),
            _ => throw new UnexpectedLexemeException(tokens.Peek())
        };
    }

    private AstNode ParseAssignmentOrExpression()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();

        if (tokens.Peek().Type == TokenType.Assign)
        {
            tokens.Advance();
            Expression value = ParseExpression();
            Match(TokenType.Semicolon);
            return new AssignmentStatement(name, value);
        }
        else
        {
            return ParseExpression();
        }
    }

    private VariableDeclaration ParseNumberVariableDeclaration()
    {
        Match(TokenType.Number);
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);

        string type = tokens.Peek().Type switch
        {
            TokenType.FloatType => "ДРОБЬ",
            TokenType.IntegerType => "ЦЕС",
            _ => throw new UnexpectedLexemeException(tokens.Peek())
        };
        tokens.Advance();

        Expression? initialValue = null;
        if (tokens.Peek().Type == TokenType.Assign)
        {
            tokens.Advance();
            initialValue = ParseExpression();
        }

        Match(TokenType.Semicolon);

        return new VariableDeclaration(name, initialValue);
    }

    private BlockStatement ParseBlock()
    {
        Match(TokenType.Begin);

        List<AstNode> statements = [];
        while (tokens.Peek().Type != TokenType.End && tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            statements.Add(node);
        }

        Match(TokenType.End);

        return new BlockStatement(statements);
    }

    private OutputStatement ParseOutput()
    {
        Match(TokenType.Output);
        Match(TokenType.LParen);

        List<object> arguments = [ParseOutputArgument()];

        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            arguments.Add(ParseOutputArgument());
        }

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new OutputStatement(arguments);
    }

    private object ParseOutputArgument()
    {
        Token token = tokens.Peek();
        if (token.Type == TokenType.StringLiteral)
        {
            string value = token.Value!.ToString();
            tokens.Advance();
            return value;
        }
        else
        {
            return ParseExpression();
        }
    }

    private InputStatement ParseInput()
    {
        Match(TokenType.Input);
        Match(TokenType.LParen);

        string variableName = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new InputStatement(variableName);
    }

    private FunctionDeclaration ParseFunctionDeclaration()
    {
        Match(TokenType.Function);
        string name = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.LParen);
        List<string> parameters = ParseParameterList();
        if (parameters.Count == 0)
        {
            throw new ArgumentException("Ur function needs at least 1 parameter");
        }

        Match(TokenType.RParen);

        Match(TokenType.Colon);

        ParseType();

        BlockStatement body = ParseBlock();
        return new FunctionDeclaration(name, parameters, body);
    }

    private List<string> ParseParameterList()
    {
        List<string> parameters = new List<string>();

        if (tokens.Peek().Type == TokenType.RParen)
        {
            return parameters;
        }

        string paramName = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        ParseType();
        parameters.Add(paramName);

        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            paramName = Match(TokenType.Identifier).Value!.ToString();
            Match(TokenType.Colon);
            ParseType();
            parameters.Add(paramName);
        }

        return parameters;
    }

    private Statement ParseReturnStatement()
    {
        Match(TokenType.Return);

        Expression returnValue = null;
        if (tokens.Peek().Type != TokenType.Semicolon)
        {
            returnValue = ParseExpression();
        }

        Match(TokenType.Semicolon);
        return new ReturnStatement(returnValue);
    }

    private string ParseType()
    {
        string typeName = tokens.Peek().Type switch
        {
            TokenType.IntegerType => "ЦЕС",
            TokenType.FloatType => "ДРОБЬ",
            _ => throw new UnexpectedLexemeException(tokens.Peek())
        };

        tokens.Advance();
        return typeName;
    }

    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    private Expression ParseLogicalOrExpression()
    {
        Expression left = ParseLogicalAndExpression();

        while (tokens.Peek().Type == TokenType.LogicalOr)
        {
            tokens.Advance();
            Expression right = ParseLogicalAndExpression();
            left = new BinaryOperationExpression(left, BinaryOperation.Or, right);
        }

        return left;
    }

    private Expression ParseLogicalAndExpression()
    {
        Expression left = ParseComparisonExpression();

        while (tokens.Peek().Type == TokenType.LogicalAnd)
        {
            tokens.Advance();
            Expression right = ParseComparisonExpression();
            left = new BinaryOperationExpression(left, BinaryOperation.And, right);
        }

        return left;
    }

    private Expression ParseComparisonExpression()
    {
        Expression left = ParseAdditiveExpression();

        if (IsComparisonOperator(tokens.Peek().Type))
        {
            BinaryOperation operation = tokens.Peek().Type switch
            {
                TokenType.Equal => BinaryOperation.Equal,
                TokenType.NotEqual => BinaryOperation.NotEqual,
                TokenType.LessThan => BinaryOperation.LessThan,
                TokenType.GreaterThan => BinaryOperation.GreaterThan,
                TokenType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
                TokenType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
                _ => throw new UnexpectedLexemeException(tokens.Peek())
            };

            tokens.Advance();
            Expression right = ParseAdditiveExpression();
            return new BinaryOperationExpression(left, operation, right);
        }

        return left;
    }

    private Expression ParseAdditiveExpression()
    {
        Expression left = ParseTermExpression();

        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    tokens.Advance();
                    Expression plusRight = ParseTermExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Add, plusRight);
                    break;
                case TokenType.MinusSign:
                    tokens.Advance();
                    Expression minusRight = ParseTermExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Substract, minusRight);
                    break;
                default:
                    return left;
            }
        }
    }

    private Expression ParseTermExpression()
    {
        Expression left = ParseFactorExpression();

        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    tokens.Advance();
                    Expression multiplyRight = ParseFactorExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Multiply, multiplyRight);
                    break;
                case TokenType.DivideSign:
                    tokens.Advance();
                    Expression divideRight = ParseFactorExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Divide, divideRight);
                    break;
                case TokenType.ModuloSign:
                    tokens.Advance();
                    Expression moduloRight = ParseFactorExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Modulo, moduloRight);
                    break;
                default:
                    return left;
            }
        }
    }

    private Expression ParseFactorExpression()
    {
        if (IsUnaryOperator(tokens.Peek().Type))
        {
            UnaryOperation operation = tokens.Peek().Type switch
            {
                TokenType.PlusSign => UnaryOperation.Plus,
                TokenType.MinusSign => UnaryOperation.Minus,
                TokenType.LogicalNot => UnaryOperation.Not,
                _ => throw new UnexpectedLexemeException(tokens.Peek())
            };

            tokens.Advance();
            Expression operand = ParseExponentiationExpression();
            return new UnaryOperationExpression(operation, operand);
        }

        return ParseExponentiationExpression();
    }

    private Expression ParseExponentiationExpression()
    {
        Expression left = ParsePrimaryExpression();

        if (tokens.Peek().Type == TokenType.ExponentiationSign)
        {
            tokens.Advance();
            Expression right = ParseExponentiationExpression();
            return new BinaryOperationExpression(left, BinaryOperation.Exponentiate, right);
        }

        return left;
    }

    private Expression ParsePrimaryExpression()
    {
        Token token = tokens.Peek();

        switch (token.Type)
        {
            case TokenType.Integer:
            case TokenType.Float:
                tokens.Advance();
                return new LiteralExpression(token.Value!.ToDecimal());

            case TokenType.True:
                tokens.Advance();
                return new LiteralExpression(1);
            case TokenType.False:
                tokens.Advance();
                return new LiteralExpression(0);
            case TokenType.Identifier:
                string name = Match(TokenType.Identifier).Value.ToString();
                if (tokens.Peek().Type == TokenType.LParen)
                {
                    return ParseFunctionCall(name);
                }
                else
                {
                    return new VariableExpression(name);
                }

            case TokenType.LParen:
                tokens.Advance();
                Expression expression = ParseExpression();
                Match(TokenType.RParen);
                return expression;

            default:
                throw new UnexpectedLexemeException(token);
        }
    }

    private Expression ParseFunctionCall(string name)
    {
        Match(TokenType.LParen);

        List<Expression> arguments = new List<Expression>();
        if (tokens.Peek().Type != TokenType.RParen)
        {
            arguments.Add(ParseExpression());
            while (tokens.Peek().Type == TokenType.Comma)
            {
                tokens.Advance();
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.RParen);
        return new FunctionCallExpression(name, arguments);
    }

    // Вспомогательные методы
    private bool IsComparisonOperator(TokenType type)
    {
        return type switch
        {
            TokenType.Equal or
            TokenType.NotEqual or
            TokenType.LessThan or
            TokenType.GreaterThan or
            TokenType.LessThanOrEqual or
            TokenType.GreaterThanOrEqual => true,
            _ => false
        };
    }

    private bool IsUnaryOperator(TokenType type)
    {
        return type switch
        {
            TokenType.PlusSign or
            TokenType.MinusSign or
            TokenType.LogicalNot => true,
            _ => false
        };
    }

    // Заглушки для остальных конструкций
    private Statement ParseIfStatement()
    {
        Match(TokenType.If);
        Match(TokenType.LParen);
        Expression condition = ParseExpression();
        Match(TokenType.RParen);

        BlockStatement thenBranch = ParseBlock();
        Statement? elseBranch = null;

        if (tokens.Peek().Type == TokenType.Else)
        {
            tokens.Advance();
            elseBranch = ParseBlock();
        }

        return new IfElseStatement(condition, thenBranch, elseBranch);
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