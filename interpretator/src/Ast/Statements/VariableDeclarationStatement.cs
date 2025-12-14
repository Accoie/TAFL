using Ast.Expressions;

using ValueType = Runtime.ValueType;

namespace Ast.Statements;

public sealed class VariableDeclarationStatement : AbstractVariableDeclaration
{
    public VariableDeclarationStatement(string name, ValueType type, Expression? value)
        : base(name)
    {
        Value = value;
        DeclaredType = type;
    }

    public ValueType DeclaredType { get; }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}