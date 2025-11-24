using Ast.Expressions;

namespace Ast.Statements;

public sealed class IfElseStatement : Statement
{
    public IfElseStatement(Expression condition, Statement thenBranch, Statement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public Statement ThenBranch { get; }

    public Statement? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}