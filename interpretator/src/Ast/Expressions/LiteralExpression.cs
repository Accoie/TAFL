namespace Ast.Expressions;

public class LiteralExpression : Expression
{
    public LiteralExpression(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}