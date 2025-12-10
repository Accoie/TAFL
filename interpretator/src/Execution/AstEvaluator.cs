using System.Globalization;

using Ast;
using Ast.Expressions;
using Ast.Statements;

using Execution;

using Runtime;

using RusMatushkaParser;

using ValueType = Runtime.ValueType;

public class AstEvaluator : IAstVisitor
{
    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly Stack<Value> values = [];

    public AstEvaluator(Context context, IEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    public Value Evaluate(AstNode node)
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

    public void Visit(LiteralExpression e)
    {
        values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        values.Push(context.TryGetValue(e.Name));
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
        Value right = values.Pop();
        Value left = values.Pop();

        switch (e.Operation)
        {
            case BinaryOperation.Add:
                HandleAdd(right, left);
                break;

            case BinaryOperation.Substract:
                HandleSubtract(right, left);
                break;

            case BinaryOperation.Multiply:
                HandleMultiply(right, left);
                break;

            case BinaryOperation.Divide:
                HandleDivide(right, left);
                break;

            case BinaryOperation.Modulo:
                HandleModulo(right, left);
                break;

            case BinaryOperation.LessThan:
                HandleLessThan(right, left);
                break;

            case BinaryOperation.GreaterThan:
                HandleGreaterThan(right, left);
                break;

            case BinaryOperation.LessThanOrEqual:
                HandleLessThanOrEqual(right, left);
                break;

            case BinaryOperation.GreaterThanOrEqual:
                HandleGreaterThanOrEqual(right, left);
                break;

            case BinaryOperation.Equal:
                values.Push(new Value(left.Equals(right)));
                break;

            case BinaryOperation.NotEqual:
                values.Push(new Value(!left.Equals(right)));
                break;

            case BinaryOperation.And:
                HandleLogicalAnd(right, left);
                break;

            case BinaryOperation.Or:
                HandleLogicalOr(right, left);
                break;

            case BinaryOperation.Exponentiate:
                HandleExponentiate(right, left);
                break;

            default:
                throw new NotImplementedException($"Unknown binary operation {e.Operation}");
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        Value value = values.Pop();
        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                HandleUnaryMinus(value);
                break;
            case UnaryOperation.Plus:
                values.Push(value);
                break;
            case UnaryOperation.Not:
                HandleLogicalNot(value);
                break;

            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(AssignmentStatement e)
    {
        e.Value.Accept(this);
        Value value = values.Peek();
        if (context.TryGetValue(e.Name).GetValueType() != value.GetValueType())
        {
            throw new TypeErrorException("Unknown types");
        }

        context.AssignVariable(e.Name, value);
    }

    public void Visit(VariableDeclarationStatement d)
    {
        Value value = new Value(false);

        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = values.Peek();
            if (d.Type != value.GetValueType())
            {
                throw new TypeErrorException("Unknown types");
            }
        }

        if (d.Value is null)
        {
            value = GetDefaultValue(d.Type);
        }

        context.DefineVariable(d.Name, value);
    }

    public void Visit(InputStatement s)
    {
        string input = environment.ReadString();
        Value value = ParseInputValue(input);
        context.AssignVariable(s.VariableName, value);
    }

    public void Visit(OutputStatement s)
    {
        foreach (Expression arg in s.Arguments)
        {
            arg.Accept(this);
            Value value = values.Pop();
            WriteValueToOutput(value);
        }

        environment.WriteLine();
    }

    public void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);

        Value conditionValue = values.Pop();

        if (conditionValue.GetValueType() != ValueType.Bool)
        {
            throw new TypeErrorException("Condition must be boolean");
        }

        if (conditionValue.AsBool())
        {
            s.ThenBranch.Accept(this);
        }
        else if (s.ElseBranch is not null)
        {
            s.ElseBranch.Accept(this);
        }
    }

    public void Visit(WhileLoopStatement whileLoopStatement)
    {
        while (true)
        {
            context.PushScope(new Scope());
            Scope lastScope = context.GetLastScope();
            lastScope.InLoop = true;

            whileLoopStatement.Condition.Accept(this);
            Value conditionValue = values.Pop();

            if (!conditionValue.AsBool())
            {
                context.PopScope();
                break;
            }

            whileLoopStatement.Body.Accept(this);

            if (lastScope.BreakState)
            {
                context.PopScope();
                break;
            }

            if (lastScope.ContinueState)
            {
                context.PopScope();
                continue;
            }

            context.PopScope();
        }
    }

    public void Visit(ForLoopStatement e)
    {
        context.PushScope(new Scope());
        Scope lastScope = context.GetLastScope();
        lastScope.InLoop = true;

        try
        {
            ExecuteForLoop(e);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(BreakStatement breakStatement)
    {
        Scope lastScope = context.GetLastScope();
        if (!lastScope.InLoop)
        {
            throw new ArgumentException("'Break' can't be out of loop");
        }

        lastScope.BreakState = true;
    }

    public void Visit(ContinueStatement continueStatement)
    {
        Scope lastScope = context.GetLastScope();
        if (!lastScope.InLoop)
        {
            throw new ArgumentException("'Continue' can't be out of loop");
        }

        lastScope.ContinueState = true;
    }

    public void Visit(FunctionDeclarationStatement d)
    {
        context.DefineFunction(d);
    }

    public void Visit(FunctionCallExpression e)
    {
        if (BuiltInFunctions.CheckBuiltInFunctions(e.Name))
        {
            ExecuteBuiltInFunction(e);
        }
        else
        {
            FunctionDeclarationStatement function = context.TryGetFunction(e.Name);

            if (function.ResultType == ValueType.Void)
            {
                throw new InvalidOperationException(
                    $"Function '{e.Name}' returns void and cannot be used as an expression");
            }

            ExecuteCustomFunction(e);
        }
    }

    public void Visit(FunctionCallStatement s)
    {
        FunctionCallExpression e = new(s.Name, s.Arguments.ToList());

        if (BuiltInFunctions.CheckBuiltInFunctions(s.Name))
        {
            ExecuteBuiltInFunction(e);
        }
        else
        {
            ExecuteCustomFunction(e);
        }
    }

    public void Visit(ReturnStatement s)
    {
        Scope lastScope = context.GetLastScope();

        if (!lastScope.InFunction)
        {
            throw new ArgumentException("'Return' can't be out of function");
        }

        if (s.Value is not null)
        {
            s.Value.Accept(this);
            if (values.Peek().GetValueType() != s.Type || s.Type == ValueType.Void)
            {
                throw new TypeErrorException("Unknown types");
            }
        }
        else if (s.Type != ValueType.Void)
        {
            throw new TypeErrorException("Unknown types");
        }

        lastScope.ReturnState = true;
    }

    public void Visit(BlockStatement s)
    {
        if (s.IsNewScope)
        {
            context.PushScope(new Scope());
        }

        foreach (AstNode b in s.Statements)
        {
            Scope lastScope = context.GetLastScope();
            if (lastScope.ReturnState && lastScope.InFunction)
            {
                break;
            }

            if (lastScope.ContinueState && lastScope.InLoop)
            {
                continue;
            }

            if (lastScope.BreakState && lastScope.InLoop)
            {
                break;
            }

            b.Accept(this);
        }

        if (s.IsNewScope)
        {
            context.PopScope();
        }
    }

    private void HandleAdd(Value right, Value left)
    {
        ValueType typeLeft = left.GetValueType();
        ValueType typeRight = right.GetValueType();

        switch ((typeLeft, typeRight))
        {
            case (ValueType.Float, ValueType.Float):
                values.Push(new Value(left.AsDecimal() + right.AsDecimal()));
                break;
            case (ValueType.String, ValueType.String):
                values.Push(new Value(left.AsString() + right.AsString()));
                break;
            default:
                throw new TypeErrorException("Unknown types");
        }
    }

    private void HandleSubtract(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            values.Push(new Value(left.AsDecimal() - right.AsDecimal()));
        }
        else
        {
            throw new TypeErrorException($"Cannot subtract types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleMultiply(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            values.Push(new Value(left.AsDecimal() * right.AsDecimal()));
        }
        else
        {
            throw new TypeErrorException($"Cannot multiply types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleDivide(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            if (right.AsDecimal() == 0)
            {
                throw new DivideByZeroException("Division by zero");
            }

            values.Push(new Value(left.AsDecimal() / right.AsDecimal()));
        }
        else
        {
            throw new TypeErrorException($"Cannot divide types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleModulo(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            if (right.AsDecimal() == 0)
            {
                throw new DivideByZeroException("Modulo by zero");
            }

            values.Push(new Value(left.AsDecimal() % right.AsDecimal()));
        }
        else
        {
            throw new TypeErrorException($"Cannot modulo types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleLessThan(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            values.Push(new Value(left.AsDecimal() < right.AsDecimal()));
        }
        else if (right.GetValueType() == ValueType.String && left.GetValueType() == ValueType.String)
        {
            values.Push(new Value(string.Compare(left.AsString(), right.AsString(), StringComparison.Ordinal) < 0));
        }
        else
        {
            throw new TypeErrorException($"Cannot compare types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleGreaterThan(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            values.Push(new Value(left.AsDecimal() > right.AsDecimal()));
        }
        else if (right.GetValueType() == ValueType.String && left.GetValueType() == ValueType.String)
        {
            values.Push(new Value(string.Compare(left.AsString(), right.AsString(), StringComparison.Ordinal) > 0));
        }
        else
        {
            throw new TypeErrorException($"Cannot compare types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleLessThanOrEqual(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            values.Push(new Value(left.AsDecimal() <= right.AsDecimal()));
        }
        else if (right.GetValueType() == ValueType.String && left.GetValueType() == ValueType.String)
        {
            values.Push(new Value(string.Compare(left.AsString(), right.AsString(), StringComparison.Ordinal) <= 0));
        }
        else
        {
            throw new TypeErrorException($"Cannot compare types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleGreaterThanOrEqual(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            values.Push(new Value(left.AsDecimal() >= right.AsDecimal()));
        }
        else if (right.GetValueType() == ValueType.String && left.GetValueType() == ValueType.String)
        {
            values.Push(new Value(string.Compare(left.AsString(), right.AsString(), StringComparison.Ordinal) >= 0));
        }
        else
        {
            throw new TypeErrorException($"Cannot compare types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleLogicalAnd(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Bool && left.GetValueType() == ValueType.Bool)
        {
            values.Push(new Value(left.AsBool() && right.AsBool()));
        }
        else
        {
            throw new TypeErrorException($"Logical AND requires boolean types, got {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleLogicalOr(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Bool && left.GetValueType() == ValueType.Bool)
        {
            values.Push(new Value(left.AsBool() || right.AsBool()));
        }
        else
        {
            throw new TypeErrorException($"Logical OR requires boolean types, got {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleExponentiate(Value right, Value left)
    {
        if (right.GetValueType() == ValueType.Float && left.GetValueType() == ValueType.Float)
        {
            double result = Math.Pow((double)left.AsDecimal(), (double)right.AsDecimal());
            values.Push(new Value((decimal)result));
        }
        else
        {
            throw new TypeErrorException($"Cannot exponentiate types {left.GetValueType()} and {right.GetValueType()}");
        }
    }

    private void HandleUnaryMinus(Value value)
    {
        if (value.GetValueType() != ValueType.Float)
        {
            throw new TypeErrorException("Unary minus requires numeric type");
        }

        values.Push(new Value(-value.AsDecimal()));
    }

    private void HandleLogicalNot(Value value)
    {
        if (value.GetValueType() != ValueType.Bool)
        {
            throw new TypeErrorException("Logical NOT requires boolean type");
        }

        values.Push(new Value(!value.AsBool()));
    }

    private void ExecuteBuiltInFunction(FunctionCallExpression e)
    {
        int count = 0;
        foreach (Expression argument in e.Arguments)
        {
            count++;
            argument.Accept(this);
        }

        List<decimal> arguments = new();
        for (int i = 0; i < count; i++)
        {
            if (values.Peek().GetValueType() != ValueType.Float)
            {
                throw new TypeErrorException("Unknown type");
            }

            arguments.Add(values.Pop().AsDecimal());
        }

        arguments.Reverse();
        object result = BuiltInFunctions.Invoke(e.Name, arguments);
        if (result is decimal d )
        {
            values.Push(new Value(d));
        }
        else if (result is string s)
        {
            values.Push(new Value(s));
        }
    }

    private void ExecuteCustomFunction(FunctionCallExpression e)
    {
        FunctionDeclarationStatement function = context.TryGetFunction(e.Name);

        if (e.Arguments.Count != function.Parameters.Count)
        {
            throw new ArgumentException(
                $"Function '{e.Name}' expects {function.Parameters.Count} arguments, " +
                $"but got {e.Arguments.Count}");
        }

        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }

        context.PushScope(new Scope());
        Scope lastScope = context.GetLastScope();
        lastScope.InFunction = true;

        foreach (Parameter parameter in Enumerable.Reverse(function.Parameters))
        {
            Value value = values.Pop();
            if (parameter.Type != value.GetValueType())
            {
                throw new TypeErrorException(
                    $"Type mismatch for parameter '{parameter.Name}'. " +
                    $"Expected: {parameter.Type}, Got: {value.GetValueType()}");
            }

            context.DefineFunctionParameter(parameter.Name, value);
        }

        function.Body.Accept(this);

        if (function.ResultType != ValueType.Void && !lastScope.ReturnState)
        {
            throw new TypeErrorException(
                $"Function '{e.Name}' must return a value of type {function.ResultType}");
        }

        context.PopScope();
    }

    private void ExecuteForLoop(ForLoopStatement e)
    {
        e.StartValue.Accept(this);
        decimal startValue = values.Pop().AsDecimal();
        CheckIsInteger(startValue);

        e.EndValue.Accept(this);
        decimal endCondition = values.Pop().AsDecimal();
        CheckIsInteger(endCondition);

        ExecuteForLoopIterations(e, startValue, endCondition);
    }

    private void ExecuteForLoopIterations(ForLoopStatement e, decimal startValue, decimal endValue)
    {
        decimal stepValue = (startValue <= endValue) ? 1.0m : -1.0m;
        decimal iteratorValue = startValue;

        context.DefineVariable(e.IteratorName, new Value(iteratorValue));

        context.PushScope(new Scope());
        Scope lastScope = context.GetLastScope();
        lastScope.InLoop = true;

        while (true)
        {
            e.Body.Accept(this);

            if (lastScope.BreakState)
            {
                break;
            }

            if (lastScope.ContinueState)
            {
                lastScope.ContinueState = false;
            }

            if (Numbers.AreEqual(iteratorValue, endValue))
            {
                break;
            }

            iteratorValue += stepValue;
            context.AssignVariable(e.IteratorName, new Value(iteratorValue));
        }

        context.PopScope();
    }

    private void CheckIsInteger(decimal d)
    {
        if (d % 1 != 0)
        {
            throw new ArgumentException($"Number '{d}' must be integer");
        }
    }

    private Value ParseInputValue(string input)
    {
        if (decimal.TryParse(input, CultureInfo.InvariantCulture, out decimal decimalValue))
        {
            return new Value(decimalValue);
        }
        else if (bool.TryParse(input, out bool boolValue))
        {
            return new Value(boolValue);
        }
        else
        {
            return new Value(input);
        }
    }

    private void WriteValueToOutput(Value value)
    {
        switch (value.GetValueType())
        {
            case ValueType.Float:
                environment.WriteNumber(value.AsDecimal());
                break;
            default:
                environment.WriteString(value.ToString());
                break;
        }
    }

    private Value GetDefaultValue(ValueType type)
    {
        return type switch
        {
            ValueType.Float => new Value(0),
            ValueType.String => new Value(""),
            ValueType.Bool => new Value(false),
            _ => throw new TypeErrorException("Unknown type")
        };
    }
}