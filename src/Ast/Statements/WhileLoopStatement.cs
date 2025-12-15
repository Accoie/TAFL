using Ast.Expressions;

namespace Ast.Statements;
public sealed class WhileLoopStatement : Statement
{
    public WhileLoopStatement(Expression condition, BlockStatement body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; init; }

    public BlockStatement Body { get; init; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
