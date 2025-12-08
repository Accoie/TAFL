using Ast.Statements;

using RusMatushkaParser;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные и другие символы).
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = [];
    private readonly Dictionary<string, FunctionDeclarationStatement> functions = [];

    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public void PopScope()
    {
        scopes.Pop();
    }

    public Scope GetLastScope()
    {
        return scopes.Peek();
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

    /// <summary>
    /// Определяет параметр функции.
    /// </summary>
    public void DefineFunctionParameter(string name, decimal? value = null)
    {
        Scope scope = scopes.Peek();

        scope.TryDefineVariable(name, value);
    }

    public FunctionDeclarationStatement TryGetFunction(string name)
    {
        if (functions.TryGetValue(name, out FunctionDeclarationStatement? function))
        {
            return function;
        }

        throw new ArgumentException($"Function '{name}' is not defined");
    }

    public void DefineFunction(FunctionDeclarationStatement function)
    {
        foreach (Scope s in scopes.Reverse())
        {
            foreach (string name in function.Parameters)
            {
                if (s.TryGetVariable(name, out decimal variable))
                {
                    throw new ArgumentException("Parameter in function is already defined");
                }
            }
        }

        if (!functions.TryAdd(function.Name, function))
        {
            throw new ArgumentException($"Function '{function.Name}' is already defined");
        }
    }

    private decimal? GetValue(string name)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryGetVariable(name, out decimal variable))
            {
                return variable;
            }
        }

        return null;
    }
}