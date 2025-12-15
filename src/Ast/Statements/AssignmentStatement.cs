using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Statements;

public class AssignmentStatement : Statement
{
    private AstAttribute<AbstractVariableDeclaration> variable;

    public AssignmentStatement(string variableName, Expression value)
    {
        Name = variableName;
        Value = value;
    }

    public string Name { get; }

    public Expression Value { get; }

    public AbstractVariableDeclaration Variable
    {
        get => variable.Get();
        set => variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}