using Ast.Expressions;

using ValueType = Runtime.ValueType;

namespace Ast.Statements;

public sealed class VariableDeclarationStatement : Statement
{
    public VariableDeclarationStatement(string name, ValueType type, Expression? value)
    {
        Name = name;
        Value = value;
        Type = type;
    }

    public string Name { get; }

    public ValueType Type { get; }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}