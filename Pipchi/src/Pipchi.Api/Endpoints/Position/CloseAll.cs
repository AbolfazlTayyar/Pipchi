using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.AccountAggregate.Specifications;
using Pipchi.Core.Enums;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.SyncedAggregates.Specifications;
using Pipchi.SharedKernel.Exceptions;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Api.Endpoints.Position
{
    public class CloseAll : EndpointWithoutRequest<Results<Ok, NotFound<string>, Conflict<string>>>
    {
        private readonly IRepository<Core.AccountAggregate.Account> _accountRepository;
        private readonly IReadRepository<Core.SyncedAggregates.Symbol> _symbolReadRepository;

        public CloseAll(IRepository<Core.AccountAggregate.Account> accountRepository,
            IReadRepository<Core.SyncedAggregates.Symbol> symbolReadRepository)
        {
            _accountRepository = accountRepository;
            _symbolReadRepository = symbolReadRepository;
        }

        public override void Configure()
        {
            Post("api/accounts/{accountId}/positions/close");
            AllowAnonymous();
            Description(x =>
                x.WithSummary("Closes all Positions")
                    .WithDescription("Closes all Positions")
                    .WithName("positions.close")
                    .WithTags("PositionEndpoints"));
        }

        public override async Task<Results<Ok, NotFound<string>, Conflict<string>>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var accountId = Route<Guid>("accountId");

            var spec = new AccountByIdWithPositionsSpecification(accountId);
            var account = await _accountRepository.FirstOrDefaultAsync(spec, cancellationToken);
            if (account == null)
                return TypedResults.NotFound($"Account with id {accountId} not found");

            var openPositions = account.Positions.Where(p => p.Status == PositionStatus.Open).ToList();
            if (openPositions.Count == 0)
                return TypedResults.Ok();

            var symbolIds = openPositions.Select(p => p.SymbolId).Distinct().ToList();
            
            var symbolsByIdsSpec = new SymbolsByIdsSpecification(symbolIds);
            var symbols = await _symbolReadRepository.ListAsync(symbolsByIdsSpec, cancellationToken);

            var symbolMap = symbols.ToDictionary(s => s.Id);

            try
            {
                account.ClosePositions(symbolMap);

                await _accountRepository.UpdateAsync(account, cancellationToken);

                return TypedResults.Ok();
            }
            catch (ConcurrencyException)
            {
                return TypedResults.Conflict("The account was modified by another request. Please retry the operation.");
            }
        }
    }
}
