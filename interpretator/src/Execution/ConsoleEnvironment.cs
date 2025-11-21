using System.Globalization;

namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    public decimal ReadNumber()
    {
        decimal.TryParse(Console.ReadLine() ?? "", out decimal n);

        return n;
    }

    public void WriteLine()
    {
        Console.WriteLine();
    }

    public void WriteNumber(decimal result)
    {
        Console.Write(result.ToString("0.#####", CultureInfo.InvariantCulture));
    }

    public void WriteString(string str)
    {
        Console.Write(str);
    }
}