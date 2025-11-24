using Ast.Expressions;

namespace Ast.Statements;

public class AssignmentStatement : Statement
{
    public AssignmentStatement(string variableName, Expression value)
    {
        Name = variableName;
        Value = value;
    }

    public string Name { get; }
    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}