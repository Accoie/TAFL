using Runtime;

namespace Execution;

public class Scope
{
    private readonly Dictionary<string, Value> variables = [];

    public bool InFunction { get; set; }

    public bool InLoop { get; set; }

    public bool ReturnState { get; set; }

    public bool ContinueState { get; set; }

    public bool BreakState { get; set; }

    /// <summary>
    /// Читает переменную из этой области видимости.
    /// Возвращает false, если переменная не объявлена в этой области видимости.
    /// </summary>
    public bool TryGetVariable(string name, out Value value)
    {
        if (variables.TryGetValue(name, out Value? v))
        {
            if (v is null)
            {
                throw new ArgumentException("Variable is not initialized");
            }

            value = v;

            return true;
        }

        value = default!;

        return false;
    }

    /// <summary>
    /// Присваивает переменную в этой области видимости.
    /// Возвращает false, если переменная не объявлена в этой области видимости.
    /// </summary>
    public bool TryAssignVariable(string name, Value value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Объявляет переменную в этой области видимости.
    /// Возвращает false, если переменная уже объявлена в этой области видимости.
    /// </summary>
    public bool TryDefineVariable(string name, Value value)
    {
        return variables.TryAdd(name, value);
    }
}