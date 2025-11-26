using Ast.Expressions;

namespace Ast.Statements;

public sealed class IfElseStatement : Statement
{
    public IfElseStatement(Expression condition, BlockStatement thenBranch, BlockStatement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public BlockStatement ThenBranch { get; }

    public BlockStatement? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}