using Ast.Expressions;

namespace Ast.Statements;

public class FunctionCallStatement : Statement
{
    private readonly List<Expression> arguments;

    public FunctionCallStatement(string name, List<Expression> arguments)
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
