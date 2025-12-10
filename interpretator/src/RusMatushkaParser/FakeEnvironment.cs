using Execution;

namespace RusMatushkaParser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly List<decimal> results = new();
    private readonly List<string> strings = new();

    public IReadOnlyList<decimal> Numbers => results;

    public IReadOnlyList<string> Strings => strings;

    public decimal ReadNumber()
    {
        return 10;
    }

    public string ReadString()
    {
        return "fake";
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
        strings.Add(str);
    }
}