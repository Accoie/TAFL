using Ast.Expressions;

namespace Ast.Statements;
public class IteratorDeclaration : AbstractVariableDeclaration
{
    public IteratorDeclaration(string name, Expression startValue)
        : base(name)
    {
        StartValue = startValue;
    }

    public Expression StartValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
