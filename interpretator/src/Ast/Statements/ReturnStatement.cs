using Ast.Expressions;

namespace Ast.Statements;

public class ReturnStatement : Statement
{
    public ReturnStatement(Expression value)
    {
        Value = value;
    }

    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}