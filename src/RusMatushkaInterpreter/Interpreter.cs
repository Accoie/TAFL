using Ast.Statements;

using Execution;

using RusMatushkaParser;

using Semantics;

namespace RusMatushkaInterpreter;

/// <summary>
/// Интерпретатор языка RusMatushka.
/// </summary>
public class Interpreter
{
    private readonly IEnvironment environment;
    private readonly BuiltInFunctions builtInFunctions = new();
    private readonly Context context;

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

        Parser parser = new(sourceCode);

        BlockStatement program = parser.ParseProgram();
        SemanticsChecker checker = new SemanticsChecker(builtInFunctions.Functions);
        checker.Check(program);

        AstEvaluator evaluator = new(context, environment);

        evaluator.Evaluate(program);
    }
}
