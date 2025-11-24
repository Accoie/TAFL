namespace Ast.Expressions;

public class FunctionCallExpression : Expression
{
    private readonly List<Expression> _arguments;

    public FunctionCallExpression(string name, List<Expression> arguments)
    {
        Name = name;
        _arguments = arguments;
    }

    public string Name { get; }

    public IReadOnlyList<Expression> Arguments => _arguments;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}