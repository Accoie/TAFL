namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
    public void WriteNumber(decimal result);

    public void WriteLine();

    public void WriteString(string str);

    public string ReadString();

    public decimal ReadNumber();
}