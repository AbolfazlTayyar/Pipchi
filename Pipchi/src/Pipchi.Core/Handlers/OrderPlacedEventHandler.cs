using MediatR;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.AccountAggregate.Specifications;
using Pipchi.Core.Events;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.SyncedAggregates.Specifications;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.Handlers;

public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IReadRepository<Symbol> _symbolRepository;

    public OrderPlacedEventHandler(IRepository<Account> accountRepository,
        IReadRepository<Symbol> symbolRepository)
    {
        _accountRepository = accountRepository;
        _symbolRepository = symbolRepository;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var orderByIdSpec = new OrderByIdSpecification(notification.OrderId);
        var order = await _accountRepository.FirstOrDefaultAsync(orderByIdSpec, cancellationToken);
        if (order == null)
            return;

        var accountByIdSpec = new AccountByIdSpecification(notification.AccountId);
        var account = await _accountRepository.FirstOrDefaultAsync(accountByIdSpec, cancellationToken);
        if (account == null)
            return;

        var symbolByIdSpec = new SymbolByIdSpecification(notification.SymbolId);
        var symbol = await _symbolRepository.FirstOrDefaultAsync(symbolByIdSpec, cancellationToken);
        if (symbol == null)
            return;

        account.AddPosition(notification.OrderId,
            notification.SymbolId,
            notification.Volume,
            notification.Type,
            notification.EntryPrice,
            notification.StopLoss,
            notification.TakeProfit);

        await _accountRepository.UpdateAsync(account, cancellationToken);
    }
}
