using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Api.Models.Order;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Api.Endpoints.Order;

public class Create : Endpoint<CreateOrderRequest, Results<Ok<CreateOrderResponse>, NotFound<string>>>
{
    private readonly IReadRepository<Account> _accountReadRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IReadRepository<Core.SyncedAggregates.Symbol> _symbolReadRepository;

    public Create(IReadRepository<Account> accountReadRepository,
        IReadRepository<Core.SyncedAggregates.Symbol> symbolReadRepository,
        IRepository<Account> accountRepository)
    {
        _accountReadRepository = accountReadRepository;
        _symbolReadRepository = symbolReadRepository;
        _accountRepository = accountRepository;
    }

    public override void Configure()
    {
        Post(CreateOrderRequest.Route);
        AllowAnonymous();
        Description(x =>
            x.WithSummary("Creates a new Order")
            .WithDescription("Creates a new Order")
            .WithName("orders.create")
            .WithTags("OrderEndpoints"));
    }

    public override async Task<Results<Ok<CreateOrderResponse>, NotFound<string>>> ExecuteAsync(CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var response = new CreateOrderResponse(request.CorrelationId);

        //if (!await _marketService.IsMarketOpenAsync(cmd.SymbolId, ct))
        //    return Result.Invalid("Market is currently closed");

        var account = await _accountReadRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
            return TypedResults.NotFound($"Account with id {request.AccountId} not found");

        var symbol = await _symbolReadRepository.GetByIdAsync(request.SymbolId, cancellationToken);
        if (symbol == null)
            return TypedResults.NotFound($"Symbol with id {request.SymbolId} not found");

        symbol.ValidatePrice(request.EntryPrice);
        symbol.ValidateVolume(request.Volume);

        var volume = new Volume(request.Volume);
        var order = account.PlaceOrder(symbol, request.Type, volume, request.EntryPrice, request.StopLoss, request.TakeProfit);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        return TypedResults.Ok(response);
    }
}
