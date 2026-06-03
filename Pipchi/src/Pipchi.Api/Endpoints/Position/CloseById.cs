using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Core.AccountAggregate.Specifications;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Api.Endpoints.Position;

public class CloseById : EndpointWithoutRequest<Results<Ok, NotFound<string>>>
{
    private readonly IRepository<Core.AccountAggregate.Account> _accountRepository;
    private readonly IReadRepository<Core.SyncedAggregates.Symbol> _symbolReadRepository;

    public CloseById(IRepository<Core.AccountAggregate.Account> accountRepository,
        IReadRepository<Core.SyncedAggregates.Symbol> symbolReadRepository)
    {
        _accountRepository = accountRepository;
        _symbolReadRepository = symbolReadRepository;
    }

    public override void Configure()
    {
        Post("api/accounts/{accountId}/positions/{positionId}/close");
        AllowAnonymous();
        Description(x =>
            x.WithSummary("Closes a Position by Id")
                .WithDescription("Closes a Position by Id")
                .WithName("positions.closeById")
                .WithTags("PositionEndpoints"));
    }

    public override async Task<Results<Ok, NotFound<string>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var accountId = Route<Guid>("accountId");
        var positionId = Route<Guid>("positionId");

        var spec = new AccountByIdWithPositionsSpecification(accountId);
        var account = await _accountRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (account == null)
            return TypedResults.NotFound($"Account with id {accountId} not found");

        var position = account.Positions.FirstOrDefault(p => p.Id == positionId);
        if (position == null)
            return TypedResults.NotFound($"Position with id {positionId} not found");

        var symbol = await _symbolReadRepository.GetByIdAsync(position.SymbolId, cancellationToken);
        if (symbol == null)
            return TypedResults.NotFound($"Symbol with id {position.SymbolId} not found");

        account.ClosePosition(positionId, symbol);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        return TypedResults.Ok();
    }
}