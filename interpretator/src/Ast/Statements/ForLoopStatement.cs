using Ast.Expressions;

namespace Ast.Statements;

public sealed class ForLoopStatement : Statement
{
    public ForLoopStatement(
        string iteratorName,
        Expression startValue,
        Expression endCondition,
        Expression? stepValue,
        Expression body
    )
    {
        IteratorName = iteratorName;
        StartValue = startValue;
        EndCondition = endCondition;
        StepValue = stepValue;
        Body = body;
    }

    public string IteratorName { get; }

    public Expression StartValue { get; }

    public Expression EndCondition { get; }

    public Expression? StepValue { get; }

    public Expression Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}