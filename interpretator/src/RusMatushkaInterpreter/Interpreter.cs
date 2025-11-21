using Execution;

using RusMatushkaParser;

namespace RusMatushkaInterpreter;

/// <summary>
/// Интерпретатор языка RusMatushka.
/// </summary>
public class Interpreter
{
    private readonly Context context;
    private readonly IEnvironment environment;

    public Interpreter(IEnvironment environment)
    {
        context = new Context();
        this.environment = environment;
    }

    /// <summary>
    /// Выполняет программу на языке RusMatushka.
    /// </summary>
    /// <param name="sourceCode">Исходный код программы.</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Parser parser = new(context, environment, sourceCode);
        parser.ParseProgram();
    }
}
