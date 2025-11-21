using Interpreter.Specs;
using Reqnroll;

[Binding]
public class InterpreterTests
{
    private readonly TestEnvironment testEnvironment;
    private readonly RusMatushkaInterpreter.Interpreter interpreter;
    private string programCode = string.Empty;

    public InterpreterTests()
    {
        testEnvironment = new TestEnvironment();
        interpreter = new RusMatushkaInterpreter.Interpreter(testEnvironment);
    }

    [When(@"я выполняю программу:")]
    public void WhenIExecuteProgram(string multilineText)
    {
        programCode = multilineText;
        testEnvironment.ClearOutput();

        interpreter.Execute(programCode);
    }

    [When(@"я ввожу в консоли:")]
    public void WhenIEnterInConsole(Table table)
    {
        testEnvironment.SetInputFromTable(table);
    }

    [Then(@"я получаю результаты:")]
    public void ThenIGetResults(string expectedOutput)
    {
        string actualOutput = testEnvironment.Output.Trim();
        string expected = expectedOutput.Trim();

        actualOutput = NormalizeLineEndings(actualOutput);
        expected = NormalizeLineEndings(expected);

        Assert.Equal(expected, actualOutput);
    }

    private string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}
