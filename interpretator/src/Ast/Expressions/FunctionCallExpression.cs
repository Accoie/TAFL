using Ast.Attributes;
using Ast.Statements;

namespace Ast.Expressions;

public class FunctionCallExpression : Expression
{
    private readonly List<Expression> arguments;
    private AstAttribute<AbstractFunctionDeclaration> function;

    public FunctionCallExpression(string name, List<Expression> arguments)
    {
        Name = name;
        this.arguments = arguments;
    }

    public string Name { get; }

    public AbstractFunctionDeclaration Function
    {
        get => function.Get();
        set => function.Set(value);
    }

    public IReadOnlyList<Expression> Arguments => arguments;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}