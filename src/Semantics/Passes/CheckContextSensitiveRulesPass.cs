using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Passes;

public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
    private readonly Stack<ExpressionContext> expressionContextStack;

    public CheckContextSensitiveRulesPass()
    {
        expressionContextStack = [];
        expressionContextStack.Push(ExpressionContext.Default);
    }

    private enum ExpressionContext
    {
        Default,
        InsideLoop,
        InsideFunction,
    }

    public override void Visit(ReturnStatement s)
    {
        if (!expressionContextStack.Contains(ExpressionContext.InsideFunction))
        {
            throw new InvalidExpressionException("'ДАРОВАТЬ' не может быть вне блока функции");
        }

        base.Visit(s);
    }

    /// <summary>
    /// Проверяет корректность программы с точки зрения использования функций.
    /// </summary>
    /// <exception cref="InvalidFunctionCallException">Бросается при неправильном вызове функций.</exception>
    public override void Visit(FunctionCallExpression e)
    {
        expressionContextStack.Push(ExpressionContext.Default);
        base.Visit(e);

        if (e.Function is BuiltInFunction)
        {
            CheckBuiltInFunctionArguments(e.Name, e.Arguments);
        }
        else if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Функция {e.Name} ожидает {e.Function.Parameters.Count} аргументов, но получено {e.Arguments.Count}"
            );
        }

        expressionContextStack.Pop();
    }

    /// <summary>
    /// Проверяет корректность программы с точки зрения использования функций.
    /// </summary>
    /// <exception cref="InvalidFunctionCallException">Бросается при неправильном вызове функций.</exception>
    public override void Visit(FunctionCallStatement e)
    {
        expressionContextStack.Push(ExpressionContext.Default);
        base.Visit(e);

        if (e.Function is BuiltInFunction)
        {
            CheckBuiltInFunctionArguments(e.Name, e.Arguments);
        }
        else if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Функция {e.Name} ожидает {e.Function.Parameters.Count} аргументов, но получено {e.Arguments.Count}"
            );
        }

        expressionContextStack.Pop();
    }

    public override void Visit(FunctionDeclarationStatement d)
    {
        expressionContextStack.Push(ExpressionContext.InsideFunction);
        try
        {
            base.Visit(d);
            BlockStatement body = d.Body;
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(WhileLoopStatement e)
    {
        expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(ForLoopStatement e)
    {
        expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(BreakStatement e)
    {
        base.Visit(e);

        if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"breakout\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(ContinueStatement e)
    {
        base.Visit(e);

        if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"contra\" expression is allowed only inside the loop");
        }
    }

    /// <summary>
    /// Проверяет типы аргументов для встроенной функции, вызываемой как оператор.
    /// </summary>
    private void CheckBuiltInFunctionArguments(string name, IReadOnlyList<Expression> arguments)
    {
        switch (name)
        {
            case "модуль":
            case "округлить":
            case "потолок":
            case "пол":
                if (arguments.Count != 1)
                {
                    throw new InvalidFunctionCallException($"Функция '{name}' ожидает 1 аргумент");
                }

                break;
            case "малое":
            case "великое":
                if (arguments.Count < 1)
                {
                    throw new InvalidFunctionCallException($"Функция '{name}' ожидает хотя бы 1 аргумент");
                }

                break;

            case "степень":
                if (arguments.Count != 2)
                {
                    throw new InvalidFunctionCallException($"Функция '{name}' ожидает 2 аргумента");
                }

                break;

            case "числовстроку":
                if (arguments.Count != 1)
                {
                    throw new InvalidFunctionCallException($"Функция '{name}' ожидает 1 аргумент");
                }

                break;

            default:
                throw new ArgumentException($"Неизвестная встроенная функция: {name}");
        }
    }
}