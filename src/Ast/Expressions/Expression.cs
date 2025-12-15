using Ast.Attributes;

namespace Ast.Expressions;

public abstract class Expression : AstNode
{
    private AstAttribute<Runtime.ValueType> resultType;

    /// <summary>
    /// Тип результата выражения.
    /// </summary>
    public Runtime.ValueType ResultType
    {
        get => resultType.Get();

        set => resultType.Set(value);
    }
}