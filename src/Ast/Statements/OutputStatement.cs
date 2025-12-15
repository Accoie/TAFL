using Ast.Expressions;

namespace Ast.Statements;

public class OutputStatement : Statement
{
    public OutputStatement(List<Expression> arguments)
    {
        Arguments = arguments;
    }

    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}