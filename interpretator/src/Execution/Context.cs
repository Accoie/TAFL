using RusMatushkaParser;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные и другие символы).
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = [];

    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public void PopScope()
    {
        scopes.Pop();
    }

    public int GetScopesCount()
    {
        return scopes.Count;
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public decimal TryGetValue(string name)
    {
        decimal? result = GetValue(name);
        if (result is null)
        {
            throw new VariableNotFoundException($"Variable '{name}' is not defined");
        }

        return result.Value;
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, decimal value)
    {
        foreach (Scope s in scopes.Reverse())
        {
            if (s.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new VariableNotFoundException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, decimal? value = null)
    {
        if (GetValue(name) is not null)
        {
            throw new ArgumentException($"Variable '{name}' is already defined");
        }

        scopes.Peek().TryDefineVariable(name, value);
    }

    private decimal? GetValue(string name)
    {
        foreach (Scope s in scopes.Reverse())
        {
            if (s.TryGetVariable(name, out decimal variable))
            {
                return variable;
            }
        }

        return null;
    }
}