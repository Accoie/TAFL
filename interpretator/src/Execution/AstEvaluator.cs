using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using RusMatushkaParser;

namespace Execution;

public class AstEvaluator : IAstVisitor
{
    private readonly Context context;

    private readonly IEnvironment environment;

    private readonly Stack<decimal> values = [];

    public AstEvaluator(Context context, IEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    public decimal Evaluate(AstNode node)
    {
        if (values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {values.Count} values: {string.Join(", ", values)}"
            );
        }

        node.Accept(this);

        return values.Count switch
        {
            0 => throw new InvalidOperationException(
                "Evaluator logical error: the stack has no evaluation result"
            ),
            > 1 => throw new InvalidOperationException(
                $"Evaluator logical error: expected 1 value, got {values.Count} values: {string.Join(", ", values)}"
            ),
            _ => values.Pop(),
        };
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
        decimal right = values.Pop();
        decimal left = values.Pop();

        switch (e.Operation)
        {
            case BinaryOperation.Add:
                values.Push(left + right);
                break;
            case BinaryOperation.Substract:
                values.Push(left - right);
                break;
            case BinaryOperation.Multiply:
                values.Push(left * right);
                break;
            case BinaryOperation.Divide:
                values.Push(left / right);
                break;
            case BinaryOperation.Modulo:
                values.Push(left % right);
                break;
            case BinaryOperation.LessThan:
                values.Push(Numbers.IsLessThan(left, right) ? 1.0m : 0.0m);
                break;
            case BinaryOperation.GreaterThan:
                values.Push(Numbers.IsGreaterThan(left, right) ? 1.0m : 0.0m);
                break;
            case BinaryOperation.LessThanOrEqual:
                values.Push(Numbers.IsLessThanOrEqual(left, right) ? 1.0m : 0.0m);
                break;
            case BinaryOperation.GreaterThanOrEqual:
                values.Push(Numbers.IsGreaterThanOrEqual(left, right) ? 1.0m : 0.0m);
                break;
            case BinaryOperation.Equal:
                values.Push(Numbers.AreEqual(left, right) ? 1.0m : 0.0m);
                break;
            case BinaryOperation.NotEqual:
                values.Push(!Numbers.AreEqual(left, right) ? 1.0m : 0.0m);
                break;
            case BinaryOperation.And:
                {
                    CheckForLogical(left);
                    CheckForLogical(right);

                    values.Push((left == 1 && right == 1) ? 1.0m : 0.0m);

                    break;
                }

            case BinaryOperation.Or:
                values.Push((left == 0 && right == 0) ? 0.0m : 1.0m);
                break;
            case BinaryOperation.Exponentiate:
                values.Push((decimal)Math.Pow((double)left, (double)right));
                break;
            default:
                throw new NotImplementedException($"Unknown binary operation {e.Operation}");
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                values.Push(-values.Pop());
                break;
            case UnaryOperation.Plus:
                break;
            case UnaryOperation.Not:
                {
                    decimal value = values.Pop();
                    CheckForLogical(value);

                    values.Push(value == 0 ? 1 : 0);
                    break;
                }

            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(LiteralExpression e)
    {
        values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        values.Push(context.TryGetValue(e.Name));
    }

    public void Visit(FunctionCallExpression e)
    {
        if (BuiltInFunctions.CheckBuiltInFunctions(e.Name))
        {
            ExecuteBuiltInFunction(e);
        }
        else
        {
            ExecuteCustomFunction(e);
        }
    }

    public void Visit(AssignmentStatement e)
    {
        // NOTE: Вычисляем выражение, и затем присваиваем его значение переменной,
        //  сохраняя результат в стеке.
        e.Value.Accept(this);
        decimal value = values.Peek();
        context.AssignVariable(e.Name, value);
    }

    public void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);

        decimal conditionValue = values.Pop();
        bool isTrueCondition = !Numbers.AreEqual(0.0m, conditionValue);
        if (isTrueCondition)
        {
            s.ThenBranch.Accept(this);
        }
        else if (s.ElseBranch is not null)
        {
            s.ElseBranch.Accept(this);
        }
    }

    public void Visit(ForLoopStatement e)
    {
        context.PushScope(new Scope());
        try
        {
            // Вычисляем начальное значение переменной-итератора.
            e.StartValue.Accept(this);
            decimal iteratorValue = values.Pop();

            // Вычисляем шаг итерации (по умолчанию 1, но может быть переопределён).
            decimal stepValue = 1.0m;
            if (e.StepValue != null)
            {
                e.StepValue.Accept(this);
                stepValue = values.Pop();
            }

            // Определяем переменную-итератор и добавляем в стек вероятное значение цикла
            context.DefineVariable(e.IteratorName, iteratorValue);
            while (true)
            {
                // Вычисляем выражение-условие и сравниваем результат с 0.
                e.EndCondition.Accept(this);
                decimal endCondition = values.Pop();
                if (Numbers.AreEqual(0.0m, endCondition))
                {
                    break;
                }

                // Выполняем тело цикла и отбрасываем результат.
                e.Body.Accept(this);
                values.Pop();

                // Выполняем инкремент итератора.
                iteratorValue += stepValue;
                context.AssignVariable(e.IteratorName, iteratorValue);
            }

            // Добавляем в стек значение 0.0, поскольку цикл хоть не возвращает осмысленного значения,
            //  но всё равно является выражением.
            values.Push(0.0m);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(VariableDeclaration d)
    {
        decimal? value = null;

        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = values.Peek();
        }

        context.DefineVariable(d.Name, value);
    }

    public void Visit(FunctionDeclaration d)
    {
        context.DefineFunction(d);
    }

    public void Visit(InputStatement s)
    {
        decimal number = environment.ReadNumber();
        context.AssignVariable(s.VariableName, number);
    }

    public void Visit(OutputStatement s)
    {
        foreach (object arg in s.Arguments)
        {
            if (arg is string str)
            {
                environment.WriteString(str);
            }
            else if (arg is Expression expr)
            {
                expr.Accept(this);
                decimal value = values.Pop();
                environment.WriteNumber(value);
            }
        }

        environment.WriteLine();
    }

    public void Visit(BlockStatement s)
    {
        context.PushScope(new Scope());

        foreach (AstNode b in s.Statements)
        {
            b.Accept(this);

            if (context.ReturnEncountered && context.IsInFunction)
            {
                break;
            }
        }

        context.PopScope();
    }

    public void Visit(ReturnStatement s)
    {
        if (s.Value != null)
        {
            s.Value.Accept(this);
        }

        context.ReturnEncountered = true;
    }

    private void ExecuteBuiltInFunction(FunctionCallExpression e)
    {
        int count = 0;
        foreach (Expression argument in e.Arguments)
        {
            count++;
            argument.Accept(this);
        }

        List<decimal> arguments = new List<decimal>();
        for (int i = 0; i < count; i++)
        {
            arguments.Add(values.Pop());
        }

        arguments.Reverse();

        values.Push(BuiltInFunctions.Invoke(e.Name, arguments));
    }

    private void ExecuteCustomFunction(FunctionCallExpression e)
    {
        FunctionDeclaration function = context.TryGetFunction(e.Name);

        bool previousReturnState = context.ReturnEncountered;
        context.ReturnEncountered = false;
        context.IsInFunction = true;

        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }

        context.PushScope(new Scope());

        foreach (string name in Enumerable.Reverse(function.Parameters))
        {
            context.DefineVariable(name, values.Pop());
        }

        function.Body.Accept(this);

        if (!context.ReturnEncountered)
        {
            throw new InvalidOperationException($"Function '{e.Name}' must return a value");
        }

        context.PopScope();

        context.ReturnEncountered = previousReturnState;
        context.IsInFunction = false;
    }

    private void CheckForLogical(decimal o)
    {
        if (o != 0 && o != 1)
        {
            throw new ArgumentException($"Number must be 0 or 1");
        }
    }
}