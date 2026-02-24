using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pipchi.Api.Models.DTOs;
using Pipchi.Api.Models.Order;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;
using Pipchi.SharedKernel.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.OrderEndpoints;

public class Create : Endpoint<CreateOrderRequest, Results<Ok<CreateOrderResponse>, NotFound<string>>>
{
    private readonly IReadRepository<Account> _accountReadRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IReadRepository<Symbol> _symbolReadRepository;
    private readonly IMapper _mapper;

    public Create(IReadRepository<Account> accountReadRepository,
        IReadRepository<Symbol> symbolReadRepository,
        IRepository<Account> accountRepository,
        IMapper mapper)
    {
        _accountReadRepository = accountReadRepository;
        _symbolReadRepository = symbolReadRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
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

        var account = await _accountReadRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
            return TypedResults.NotFound($"Account with id {request.AccountId} not found");

        var symbol = await _symbolReadRepository.GetByIdAsync(request.SymbolId, cancellationToken);
        if (symbol == null)
            return TypedResults.NotFound($"Symbol with id {request.SymbolId} not found");

        var order = account.PlaceOrder(symbol, request.Type, request.Volume, request.EntryPrice, request.StopLoss, request.TakeProfit);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        response.OrderDto = _mapper.Map<OrderDto>(order);

        return TypedResults.Ok(response);
    }
}
