using Ast;
using Ast.Expressions;
using Ast.Statements;

using Execution;

using Runtime;

using ValueType = Runtime.ValueType;

namespace RusMatushkaParser;

#pragma warning disable SA1629 // Ложное срабатывание на знак '<'

/// <summary>
/// Выполняет синтаксический разбор и строит AST.
/// </summary>
public class Parser
{
    private readonly Context context;
    private readonly TokenStream tokens;
    private readonly AstEvaluator evaluator;

    private readonly Stack<ValueType> returnTypes = new();

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
        evaluator.Visit(ParseBlock(true));
        if (context.GetScopesCount() != 0)
        {
            throw new ArgumentException("Program's scope is not closed");
        }
    }

    /// <summary>
    /// Разбирает инструкцию верхнего уровня.
    /// Правило: statement = variable_declaration | function_declaration | assignment_statement | for_statement |
    /// output_statement | input_statement | break_statement | continue_statement | return_statement |
    /// if_statement | while_statement | expression_statement | block.
    /// </summary>
    private Statement ParseStatement()
    {
        TokenType token = tokens.Peek().Type;

        return token switch
        {
            TokenType.Identifier => ParseAssignmentOrFunctionCall(),
            TokenType.Number => ParseNumberVariableDeclaration(),
            TokenType.BooleanType => ParseStringOrBoolVariableDeclaration(false),
            TokenType.StringType => ParseStringOrBoolVariableDeclaration(true),
            TokenType.Begin => ParseBlock(true),
            TokenType.Input => ParseInput(),
            TokenType.Output => ParseOutput(),
            TokenType.Function => ParseFunctionDeclaration(),
            TokenType.If => ParseIfStatement(),
            TokenType.Return => ParseReturnStatement(),
            TokenType.While => ParseWhileLoopStatement(),
            TokenType.For => ParseForLoopStatement(),
            TokenType.Break => ParseBreakStatement(),
            TokenType.Continue => ParseContinueStatement(),
            _ => throw new UnexpectedLexemeException(tokens.Peek())
        };
    }

    private VariableDeclarationStatement ParseStringOrBoolVariableDeclaration(bool isString)
    {
        if (isString)
        {
            Match(TokenType.StringType);
        }
        else
        {
            Match(TokenType.BooleanType);
        }

        string name = Match(TokenType.Identifier).Value!.ToString();

        Expression? initialValue = null;
        if (tokens.Peek().Type == TokenType.Assign)
        {
            tokens.Advance();
            initialValue = ParseExpression();
        }

        Match(TokenType.Semicolon);

        return new VariableDeclarationStatement(name, isString ? ValueType.String : ValueType.Bool, initialValue);
    }

    /// <summary>
    /// Разбирает цикл for.
    /// Правило: for_statement = "ДЛЯ", identifier, "ОТ", expression, "ДО", expression, "ТВОРИ", statement.
    /// </summary>
    private ForLoopStatement ParseForLoopStatement()
    {
        Match(TokenType.For);

        string iteratorName = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.From);
        Expression startExpression = ParseExpression();

        Match(TokenType.To);
        Expression endExpression = ParseExpression();

        Match(TokenType.Do);

        Statement body = ParseBlock(false);

        return new ForLoopStatement(iteratorName, startExpression, endExpression, body);
    }

    /// <summary>
    /// Разбирает оператор continue.
    /// Правило: continue_statement = "ПРОДОЛЖИТЬ", ";".
    /// </summary>
    private ContinueStatement ParseContinueStatement()
    {
        Match(TokenType.Continue);
        Match(TokenType.Semicolon);

        return new ContinueStatement();
    }

    /// <summary>
    /// Разбирает оператор break.
    /// Правило: break_statement = "ВЫЙТИ", ";".
    /// </summary>
    private BreakStatement ParseBreakStatement()
    {
        Match(TokenType.Break);
        Match(TokenType.Semicolon);

        return new BreakStatement();
    }

    /// <summary>
    /// Разбирает цикл while.
    /// Правило: while_statement = "ПОКУДА", "(", expression, ")", "ТВОРИ", statement.
    /// </summary>
    private WhileLoopStatement ParseWhileLoopStatement()
    {
        Match(TokenType.While);
        Match(TokenType.LParen);
        Expression condition = ParseExpression();
        Match(TokenType.RParen);
        Match(TokenType.Do);

        Statement body = ParseBlock(false);

        return new WhileLoopStatement(condition, body);
    }

    /// <summary>
    /// Разбирает оператор присваивания.
    /// Правило: assignment_statement = identifier, "=", expression, ";".
    /// </summary>
    private Statement ParseAssignmentOrFunctionCall()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Statement result;
        if (tokens.Peek().Type == TokenType.Assign)
        {
            Match(TokenType.Assign);
            Expression value = ParseExpression();
            result = new AssignmentStatement(name, value);
        }
        else
        {
            result = ParseFunctionCallStatement(name);
        }

        Match(TokenType.Semicolon);

        return result;
    }

    /// <summary>
    /// Разбирает объявление числовой переменной.
    /// Правило: variable_declaration = "ЧИСЛО", identifier, ":", type, [ "=", expression ], ";".
    /// </summary>
    private VariableDeclarationStatement ParseNumberVariableDeclaration()
    {
        Match(TokenType.Number);
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);

        string type = tokens.Peek().Type switch
        {
            TokenType.FloatType => "ДРОБЬ",
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

        return new VariableDeclarationStatement(name, ValueType.Float, initialValue);
    }

    /// <summary>
    /// Разбирает блок кода.
    /// Правило: block = "НАЧАЛО", { statement }, "ИСХОД".
    /// </summary>
    private BlockStatement ParseBlock(bool isNew)
    {
        Match(TokenType.Begin);

        List<Statement> statements = [];
        while (tokens.Peek().Type != TokenType.End && tokens.Peek().Type != TokenType.EndOfFile)
        {
            Statement node = ParseStatement();
            statements.Add(node);
        }

        Match(TokenType.End);

        return new BlockStatement(statements, isNew);
    }

    /// <summary>
    /// Разбирает оператор вывода.
    /// Правило: output_statement = "МОЛВИ", "(", argument_list, ")", ";".
    /// </summary>
    private OutputStatement ParseOutput()
    {
        Match(TokenType.Output);
        Match(TokenType.LParen);

        List<Expression> arguments = [ParseExpression()];

        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            arguments.Add(ParseExpression());
        }

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new OutputStatement(arguments);
    }

    /// <summary>
    /// Разбирает оператор ввода.
    /// Правило: input_statement = "ВНЕМЛИ", "(", identifier, ")", ";".
    /// </summary>
    private InputStatement ParseInput()
    {
        Match(TokenType.Input);
        Match(TokenType.LParen);

        string variableName = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new InputStatement(variableName);
    }

    /// <summary>
    /// Разбирает объявление функции.
    /// Правило: function_declaration = "ФУНКЦИЯ", function_name, "(", [ parameter_list ], ")", [ ":", type ], block.
    /// </summary>
    private FunctionDeclarationStatement ParseFunctionDeclaration()
    {
        Match(TokenType.Function);
        string name = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.LParen);
        List<Parameter> parameters = ParseParameterList();
        if (parameters.Count == 0)
        {
            throw new ArgumentException("Function needs at least 1 parameter");
        }

        Match(TokenType.RParen);

        TokenType returnTypetokens = tokens.Peek().Type;
        ValueType resultType;
        if (returnTypetokens == TokenType.Colon)
        {
            Match(TokenType.Colon);
            resultType = ParseType();
        }
        else
        {
            resultType = ValueType.Void;
        }

        returnTypes.Push(resultType);

        BlockStatement body = ParseBlock(false);

        returnTypes.Pop();

        return new FunctionDeclarationStatement(name, parameters, body, resultType);
    }

    /// <summary>
    /// Разбирает вызов функции.
    /// Правило: function_call = function_name, "(", [ argument_list ], ")".
    /// </summary>
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

    /// <summary>
    /// Разбирает вызов функции.
    /// Правило: function_call = function_name, "(", [ argument_list ], ")".
    /// </summary>
    private Statement ParseFunctionCallStatement(string name)
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

        return new FunctionCallStatement(name, arguments);
    }

    /// <summary>
    /// Разбирает условный оператор if.
    /// Правило: if_statement = "ЕСЛИ", "(", expression, ")", "СТАЛОБЫТЬ", statement, [ "ИНО", statement ].
    /// </summary>
    private Statement ParseIfStatement()
    {
        Match(TokenType.If);
        Match(TokenType.LParen);
        Expression condition = ParseExpression();
        Match(TokenType.RParen);
        Match(TokenType.Then);
        BlockStatement thenBranch = ParseBlock(false);
        BlockStatement? elseBranch = null;

        if (tokens.Peek().Type == TokenType.Else)
        {
            tokens.Advance();
            elseBranch = ParseBlock(false);
        }

        return new IfElseStatement(condition, thenBranch, elseBranch);
    }

    /// <summary>
    /// Разбирает список параметров функции.
    /// Правило: parameter_list = parameter, { ",", parameter }.
    /// </summary>
    private List<Parameter> ParseParameterList()
    {
        List<Parameter> parameters = new List<Parameter>();

        if (tokens.Peek().Type == TokenType.RParen)
        {
            return parameters;
        }

        string paramName = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        parameters.Add(new Parameter(paramName, ParseType()));

        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            paramName = Match(TokenType.Identifier).Value!.ToString();
            Match(TokenType.Colon);
            parameters.Add(new Parameter(paramName, ParseType()));
        }

        return parameters;
    }

    /// <summary>
    /// Разбирает оператор возврата.
    /// Правило: return_statement = "ДАРОВАТЬ", [ expression ], ";".
    /// </summary>
    private Statement ParseReturnStatement()
    {
        Match(TokenType.Return);
        if (tokens.Peek().Type == TokenType.Semicolon)
        {
            Match(TokenType.Semicolon);
            return new ReturnStatement(null, returnTypes.Peek());
        }

        Expression returnValue;

        returnValue = ParseExpression();

        Match(TokenType.Semicolon);

        return new ReturnStatement(returnValue, returnTypes.Peek() );
    }

    /// <summary>
    /// Разбирает тип данных.
    /// Правило: type = "ЦЕС" | "ДРОБЬ".
    /// </summary>
    private ValueType ParseType()
    {
        ValueType type = tokens.Peek().Type switch
        {
            TokenType.FloatType => ValueType.Float,
            TokenType.StringType => ValueType.String,
            TokenType.BooleanType => ValueType.Bool,
            _ => throw new UnexpectedLexemeException(tokens.Peek())
        };

        tokens.Advance();
        return type;
    }

    /// <summary>
    /// Разбирает выражение.
    /// Правило: expression = logical_or_expression.
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает логическое ИЛИ выражение.
    /// Правило: logical_or_expression = logical_and_expression, { logical_or_operator, logical_and_expression }.
    /// </summary>
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

    /// <summary>
    /// Разбирает логическое И выражение.
    /// Правило: logical_and_expression = comparison_expression, { logical_and_operator, comparison_expression }.
    /// </summary>
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

    /// <summary>
    /// Разбирает выражение сравнения.
    /// Правило: comparison_expression = additive_expression, [ comparison_operator, additive_expression ].
    /// </summary>
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

    /// <summary>
    /// Разбирает аддитивное выражение (сложение/вычитание).
    /// Правило: additive_expression = term_expression, { additive_operator, term_expression }.
    /// </summary>
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

    /// <summary>
    /// Разбирает мультипликативное выражение (умножение/деление/остаток).
    /// Правило: term_expression = factor_expression, { multiplicative_operator, factor_expression }.
    /// </summary>
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

    /// <summary>
    /// Разбирает унарные операции и факторные выражения.
    /// Правило: factor_expression = [ unary_operator ], exponentiation_expression.
    /// </summary>
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

    /// <summary>
    /// Разбирает выражение возведения в степень.
    /// Правило: exponentiation_expression = primary_expression, [ "^", exponentiation_expression ].
    /// </summary>
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

    /// <summary>
    /// Разбирает первичные выражения (литералы, идентификаторы, вызовы функций, выражения в скобках).
    /// Правило: primary_expression = literal | variable_access | function_call | "(", expression, ")".
    /// </summary>
    private Expression ParsePrimaryExpression()
    {
        Token token = tokens.Peek();

        switch (token.Type)
        {
            case TokenType.Integer:
            case TokenType.Float:
                tokens.Advance();
                return new LiteralExpression(new Value(token.Value!.ToDecimal()));
            case TokenType.True:
                tokens.Advance();
                return new LiteralExpression(new Value(true));
            case TokenType.False:
                tokens.Advance();
                return new LiteralExpression(new Value(false));
            case TokenType.StringLiteral:
                tokens.Advance();
                return new LiteralExpression(new Value(token.Value!.ToString()));
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

    /// <summary>
    /// Проверяет, является ли токен оператором сравнения.
    /// Правило: comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=".
    /// </summary>
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

    /// <summary>
    /// Проверяет, является ли токен унарным оператором.
    /// Правило: unary_operator = "+" | "-" | "!".
    /// </summary>
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

#pragma warning restore SA1629