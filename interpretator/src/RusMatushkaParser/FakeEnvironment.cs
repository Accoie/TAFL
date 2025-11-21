using Execution;

namespace RusMatushkaParser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly List<decimal> results = [];

    public IReadOnlyList<decimal> Results => results;

    public decimal ReadNumber()
    {
        return 10;
    }

    public void WriteLine()
    {
    }

    public void WriteNumber(decimal result)
    {
        results.Add(result);
    }

    public void WriteString(string str)
    {
    }
}