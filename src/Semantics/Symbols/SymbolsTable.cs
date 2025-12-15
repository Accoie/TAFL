using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Symbols;

/// <summary>
/// Таблица символов, основанная на лексических областях видимости (областях действия) символов в коде.
/// </summary>
public sealed class SymbolsTable
{
    private readonly SymbolsTable? parent;

    private readonly Dictionary<string, DeclarationStatement> symbols;

    public SymbolsTable(SymbolsTable? parent)
    {
        this.parent = parent;
        symbols = [];
    }

    public SymbolsTable? Parent => parent;

    public DeclarationStatement GetSymbol(string name)
    {
        if (symbols.TryGetValue(name, out DeclarationStatement? symbol))
        {
            return symbol;
        }

        if (parent != null)
        {
            return parent.GetSymbol(name);
        }

        throw new UnknownSymbolException(name);
    }

    public void DefineSymbol(string name, DeclarationStatement symbol)
    {
        if (!symbols.TryAdd(name, symbol))
        {
            throw new DuplicateSymbolException(name);
        }
    }
}