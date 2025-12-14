namespace Ast.Statements;

/// <summary>
/// Абстрактный класс с информацией о функции — как встроенной, так и пользовательской.
/// </summary>
public abstract class AbstractFunctionDeclaration : DeclarationStatement
{
    protected AbstractFunctionDeclaration(
        string name,
        IReadOnlyList<AbstractParameterDeclaration> parameters
    )
    {
        Name = name;
        Parameters = parameters;
    }

    public string Name { get; }

    public IReadOnlyList<AbstractParameterDeclaration> Parameters { get; }
}