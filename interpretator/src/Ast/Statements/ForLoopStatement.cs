using Ast.Expressions;

namespace Ast.Statements;

public sealed class ForLoopStatement : Statement
{
    public ForLoopStatement(
        string iteratorName,
        Expression startValue,
        Expression endCondition,
        Statement body
    )
    {
        IteratorName = iteratorName;
        StartValue = startValue;
        EndValue = endCondition;
        Body = body;
    }

    public string IteratorName { get; }

    public Expression StartValue { get; }

    public Expression EndValue { get; }

    public Statement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}