using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Api.Models.Account;
using Pipchi.Api.Models.Position;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.AccountAggregate.Specifications;
using Pipchi.SharedKernel.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.Endpoints.Position;

public class Edit : Endpoint<EditPositionRequest, Results<Ok<EditPositionResponse>, NotFound<string>>>
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IMapper _mapper;

    public Edit(IRepository<Account> accountRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put(EditPositionRequest.Route);
        AllowAnonymous();
        Description(x =>
            x.WithSummary("Updates a Position")
                .WithDescription("Updates a Position")
                .WithName("positions.update")
                .WithTags("PositionEndpoints"));
    }

    public override async Task<Results<Ok<EditPositionResponse>, NotFound<string>>> ExecuteAsync(EditPositionRequest request, CancellationToken cancellationToken)
    {
        var response = new EditPositionResponse(request.CorrelationId);

        var spec = new AccountByIdWithPositionsSpecification(request.AccountId);
        var account = await _accountRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (account == null)
            return TypedResults.NotFound($"Account with id {request.AccountId} not found");

        account.UpdatePosition(request.PositionId, request.StopLoss, request.TakeProfit);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        response.PositionDto = _mapper.Map<PositionDto>(account.Positions.First(p => p.Id == request.PositionId));

        return TypedResults.Ok(response);
    }
}
