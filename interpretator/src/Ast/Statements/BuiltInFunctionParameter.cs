namespace Ast.Statements;

/// <summary>
/// Объявляет параметр встроенной функции.
/// </summary>
public class BuiltInFunctionParameter : AbstractParameterDeclaration
{
    public BuiltInFunctionParameter(string name, Runtime.ValueType type)
        : base(name)
    {
        ResultType = type;
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}