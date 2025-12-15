namespace Ast.Statements;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : AbstractParameterDeclaration
{
    public ParameterDeclaration(string name, Runtime.ValueType type)
        : base(name)
    {
        ResultType = type;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}