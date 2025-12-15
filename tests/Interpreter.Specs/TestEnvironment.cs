using Execution;

using Reqnroll;

namespace Interpreter.Specs;

/// <summary>
/// Имитирует поведение консоли.
/// </summary>
public class TestEnvironment : IEnvironment
{
    private readonly Queue<string> inputQueue = new Queue<string>();

    public string Output { get; private set; } = string.Empty;

    public void SetInputValues(params string[] values)
    {
        inputQueue.Clear();
        foreach (string value in values)
        {
            inputQueue.Enqueue(value);
        }
    }

    public void SetInputFromTable(Table table)
    {
        inputQueue.Clear();
        foreach (DataTableRow? row in table.Rows)
        {
            inputQueue.Enqueue(row["Value"]);
        }
    }

    public void SetInputNumbers(params decimal[] values)
    {
        inputQueue.Clear();
        foreach (decimal value in values)
        {
            inputQueue.Enqueue(value.ToString());
        }
    }

    public decimal ReadNumber()
    {
        string input = ReadString();
        if (decimal.TryParse(input, out decimal result))
        {
            return result;
        }

        throw new InvalidOperationException($"Не удалось преобразовать '{input}' в число.");
    }

    public void WriteLine()
    {
        Output += Environment.NewLine;
    }

    public void WriteNumber(decimal result)
    {
        string formatted = result % 1 == 0 ?
            result.ToString("0") :
            result.ToString("0.00");
        Output += formatted;
    }

    public void WriteString(string str)
    {
        Output += str;
    }

    public void ClearOutput()
    {
        Output = string.Empty;
    }

    public string ReadString()
    {
        if (inputQueue.Count > 0)
        {
            return inputQueue.Dequeue();
        }

        throw new InvalidOperationException("Нет данных для ввода. Проверьте настройку тестовых данных.");
    }
}