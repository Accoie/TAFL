using Ast.Statements;

using Semantics.Passes;
using Semantics.Symbols;

namespace Semantics;

/// <summary>
/// Фасад для проведения семантических проверок программы.
/// Выполняет три прохода по AST:
/// 1. ResolveNamesPass - разрешение имен и проверка областей видимости
/// 2. ResolveTypesPass - проверка типов данных
/// 3. CheckSemanticsRulesPass - проверка семантических правил.
/// </summary>
public class SemanticsChecker
{
    private readonly AbstractPass[] passes;

    public SemanticsChecker(
        IReadOnlyDictionary<string, BuiltInFunction> builtinFunctions
    )
    {
        SymbolsTable globalSymbols = new(parent: null);
        foreach ((string name, BuiltInFunction function) in builtinFunctions)
        {
            globalSymbols.DefineSymbol(name, function);
        }

        passes =
        [
            new ResolveNamesPass(globalSymbols),
            new CheckContextSensitiveRulesPass(),
            new ResolveTypesPass(),
        ];
    }

    public void Check(BlockStatement program)
    {
        foreach (AbstractPass pass in passes)
        {
            program.Accept(pass);
        }
    }
}