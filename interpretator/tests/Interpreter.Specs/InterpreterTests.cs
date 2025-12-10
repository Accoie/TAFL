using Interpreter.Specs;

using Reqnroll;

[Binding]
public class InterpreterTests
{
    private readonly TestEnvironment testEnvironment;
    private readonly RusMatushkaInterpreter.Interpreter interpreter;
    private readonly ScenarioContext scenarioContext;
    private string programCode = string.Empty;

    public InterpreterTests(TestEnvironment testEnvironment, ScenarioContext scenarioContext)
    {
        this.testEnvironment = testEnvironment;
        this.scenarioContext = scenarioContext;
        interpreter = new RusMatushkaInterpreter.Interpreter(testEnvironment);
    }

    [When(@"я выполняю программу:")]
    public void WhenIExecuteProgram(string multilineText)
    {
        try
        {
            programCode = multilineText;
            testEnvironment.ClearOutput();
            interpreter.Execute(programCode);
            scenarioContext["ExecutionResult"] = "Success";
            scenarioContext["Output"] = testEnvironment.Output;
        }
        catch (Exception ex)
        {
            scenarioContext["ExecutionException"] = ex;
            scenarioContext["ExceptionType"] = ex.GetType().Name;
            scenarioContext["ExceptionMessage"] = ex.Message;
        }
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

    [Then(@"программа выполняется успешно")]
    public void ThenProgramExecutesSuccessfully()
    {
        Assert.False(
            scenarioContext.ContainsKey("ExecutionException"),
            $"Ошибка: {scenarioContext.Get<Exception>("ExecutionException")?.Message}");
    }

    [Then(@"я получаю ошибку типа ""(.*)""")]
    public void ThenIGetErrorOfType(string expectedExceptionType)
    {
        Assert.True(
            scenarioContext.ContainsKey("ExecutionException"),
            "Ожидалось исключение, но программа выполнилась успешно");

        Exception exception = scenarioContext.Get<Exception>("ExecutionException");
        string actualExceptionType = exception.GetType().Name;
        Assert.Equal(expectedExceptionType, actualExceptionType);
    }

    private string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}