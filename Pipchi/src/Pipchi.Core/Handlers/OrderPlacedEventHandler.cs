using MediatR;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.AccountAggregate.Specifications;
using Pipchi.Core.Events;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.Handlers;

public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IRepository<Account> _accountRepository;

    public OrderPlacedEventHandler(IRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var accountByIdSpec = new AccountByIdSpecification(notification.AccountId);
        var account = await _accountRepository.FirstOrDefaultAsync(accountByIdSpec, cancellationToken);
        if (account == null)
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
