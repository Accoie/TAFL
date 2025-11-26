namespace Ast.Expressions;

public class FunctionCallExpression : Expression
{
    private readonly List<Expression> arguments;

    public FunctionCallExpression(string name, List<Expression> arguments)
    {
        Name = name;
        this.arguments = arguments;
    }

    public string Name { get; }

    public IReadOnlyList<Expression> Arguments => arguments;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}