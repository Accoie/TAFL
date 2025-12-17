using System;

using Ast.Statements;

using Execution.Exceptions;

using Runtime;

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
    public Value TryGetValue(string name)
    {
        Value? result = GetValue(name);
        if (result is null)
        {
            throw new VariableNotFoundException($"Variable '{name}' is not defined");
        }

        return result;
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, Value value)
    {
        foreach (Scope s in scopes)
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
    public void DefineVariable(string name, Value value)
    {
        if (!scopes.Peek().TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Variable '{name}' is already defined");
        }
    }

    /// <summary>
    /// Определяет параметр функции.
    /// </summary>
    public void DefineFunctionParameter(string name, Value value)
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
        if (!functions.TryAdd(function.Name, function))
        {
            throw new ArgumentException($"Function '{function.Name}' is already defined");
        }
    }

    private Value? GetValue(string name)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryGetVariable(name, out Value variable))
            {
                return variable;
            }
        }

        return null;
    }
}