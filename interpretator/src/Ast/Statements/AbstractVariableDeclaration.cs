namespace Ast.Statements;

/// <summary>
/// Абстрактный класс с информацией о переменной или формальном параметре функции.
/// </summary>
public abstract class AbstractVariableDeclaration : DeclarationStatement
{
    protected AbstractVariableDeclaration(string name)
    {
        this.Name = name;
    }

    public string Name { get; }
}