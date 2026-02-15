namespace Pipchi.Core.Interfaces;

public interface ISymbolUniquenessChecker
{
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}