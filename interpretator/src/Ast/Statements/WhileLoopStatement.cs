using Ast.Expressions;

namespace Ast.Statements;
public sealed class WhileLoopStatement : Statement
{
    public WhileLoopStatement(Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; init; }

    public Statement Body { get; init; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
