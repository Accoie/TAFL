using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly Stack<FunctionDeclarationStatement> functionStack;

    private SymbolsTable symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        symbols = globalSymbols;
        functionStack = new Stack<FunctionDeclarationStatement>();
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        try
        {
            DeclarationStatement symbol = symbols.GetSymbol(e.Name);

            if (!(symbol is AbstractVariableDeclaration))
            {
                throw new InvalidSymbolException(
                    $"Имя '{e.Name}' не ссылается на переменную"
                );
            }

            e.Variable = (AbstractVariableDeclaration)symbol;
        }
        catch (UnknownSymbolException)
        {
            throw new UnknownSymbolException($"Неизвестная переменная '{e.Name}'");
        }
    }

    public override void Visit(InputStatement s)
    {
        DeclarationStatement symbol = symbols.GetSymbol(s.VariableName);
        base.Visit(s);
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);

        if (!IsBuiltInFunction(e.Name))
        {
            try
            {
                DeclarationStatement symbol = symbols.GetSymbol(e.Name);

                if (symbol is FunctionDeclarationStatement function)
                {
                    e.Function = function;
                }
                else
                {
                    throw new InvalidSymbolException(
                        $"Имя '{e.Name}' не ссылается на функцию"
                    );
                }
            }
            catch (UnknownSymbolException)
            {
                throw new UnknownSymbolException($"Неизвестная функция '{e.Name}'");
            }
        }
        else
        {
            e.Function = (BuiltInFunction)symbols.GetSymbol(e.Name);
        }
    }

    public override void Visit(FunctionCallStatement s)
    {
        base.Visit(s);

        if (!IsBuiltInFunction(s.Name))
        {
            DeclarationStatement symbol = symbols.GetSymbol(s.Name);

            if (symbol is AbstractFunctionDeclaration function)
            {
                if (s.Arguments.Count != function.Parameters.Count)
                {
                    throw new InvalidFunctionCallException(
                        $"Функция '{s.Name}' ожидает {function.Parameters.Count} аргументов, " +
                        $"но получено {s.Arguments.Count}"
                    );
                }

                s.Function = function;
            }
            else
            {
                throw new InvalidSymbolException(
                    $"Имя '{s.Name}' не ссылается на функцию"
                );
            }
        }
    }

    public override void Visit(BlockStatement s)
    {
        if (s.IsNewScope)
        {
            symbols = new SymbolsTable(symbols);
        }

        try
        {
            PredeclareFunctions(s.Statements);
            ProcessDeclarationsAndStatements(s.Statements);
        }
        finally
        {
            if (s.IsNewScope)
            {
                symbols = symbols.Parent!;
            }
        }
    }

    public override void Visit(VariableDeclarationStatement d)
    {
        base.Visit(d);

        if (IsBuiltInFunction(d.Name))
        {
            throw new DuplicateSymbolException(d.Name);
        }

        symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(FunctionDeclarationStatement d)
    {
        symbols = new SymbolsTable(symbols);
        try
        {
            foreach (ParameterDeclaration parameter in d.Parameters)
            {
                parameter.Accept(this);
            }

            functionStack.Push(d);
            d.Body.Accept(this);
            functionStack.Pop();
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);

        try
        {
            symbols.DefineSymbol(d.Name, d);
        }
        catch (DuplicateSymbolException)
        {
            throw new DuplicateSymbolException(
                $"Параметр '{d.Name}' уже объявлен в этой функции"
            );
        }
    }

    public override void Visit(ForLoopStatement s)
    {
        symbols = new SymbolsTable(symbols);
        try
        {
            base.Visit(s);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(IteratorDeclaration e)
    {
        base.Visit(e);

        symbols.DefineSymbol(e.Name, e);
    }

    public override void Visit(WhileLoopStatement s)
    {
        symbols = new SymbolsTable(symbols);
        try
        {
            base.Visit(s);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);

        symbols = new SymbolsTable(symbols);
        try
        {
            s.ThenBranch.Accept(this);
        }
        finally
        {
            symbols = symbols.Parent!;
        }

        if (s.ElseBranch != null)
        {
            symbols = new SymbolsTable(symbols);
            try
            {
                s.ElseBranch.Accept(this);
            }
            finally
            {
                symbols = symbols.Parent!;
            }
        }
    }

    public override void Visit(AssignmentStatement s)
    {
        base.Visit(s);
        DeclarationStatement symbol = symbols.GetSymbol(s.Name);

        if (symbol is not AbstractVariableDeclaration)
        {
            throw new InvalidSymbolException(
                $"Имя '{s.Name}' не ссылается на переменную"
            );
        }

        s.Variable = (AbstractVariableDeclaration)symbol;
    }

    public override void Visit(ReturnStatement s)
    {
        base.Visit(s);

        if (functionStack.Count == 0)
        {
            throw new InvalidOperationException("Оператор 'ДАРОВАТЬ' не может находиться вне функции");
        }
    }

    private void PredeclareFunctions(IEnumerable<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is FunctionDeclarationStatement function)
            {
                try
                {
                    symbols.DefineSymbol(function.Name, function);
                }
                catch (DuplicateSymbolException)
                {
                    throw new DuplicateSymbolException(
                        $"Функция '{function.Name}' уже объявлена в этой области видимости"
                    );
                }
            }
        }
    }

    private void ProcessDeclarationsAndStatements(IEnumerable<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is VariableDeclarationStatement variable)
            {
                variable.Accept(this);
            }
        }

        foreach (Statement statement in statements)
        {
            if (statement is VariableDeclarationStatement variable)
            {
            }
            else if (statement is FunctionDeclarationStatement function)
            {
                function.Accept(this);
            }
            else
            {
                statement.Accept(this);
            }
        }
    }

    private bool IsBuiltInFunction(string name)
    {
        string[] builtInFunctions =
        {
            "модуль", "малое", "великое", "округлить",
            "потолок", "пол", "степень", "числовстроку",
        };

        foreach (string builtIn in builtInFunctions)
        {
            if (string.Equals(builtIn, name))
            {
                return true;
            }
        }

        return false;
    }
}