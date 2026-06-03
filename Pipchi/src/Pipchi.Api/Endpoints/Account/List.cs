using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Api.Models.Account;
using Pipchi.Core.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.Endpoints.Account;

public class List : Endpoint<ListAccountRequest, Results<Ok<ListAccountResponse>, NotFound>>
{
    private readonly IAccountRepository _repository;
    private readonly IMapper _mapper;

    public List(IAccountRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get(ListAccountRequest.Route);
        AllowAnonymous();
        Description(d =>
            d.WithSummary("List Accounts")
             .WithDescription("List Accounts")
             .WithName("accounts.List")
             .WithTags("AccountEndpoints"));
    }

    public override async Task<Results<Ok<ListAccountResponse>, NotFound>> ExecuteAsync(
        ListAccountRequest req,
        CancellationToken ct)
    {
        var response = new ListAccountResponse(req.CorrelationId);

        var accounts = await _repository.ListWithKeysetPaginationAsync(req.LastCreatedAt,
                                                                       req.LastId,
                                                                       req.PageSize ?? 20,
                                                                       ct);
        if (accounts is null) return TypedResults.NotFound();

        response.Accounts = _mapper.Map<List<AccountDto>>(accounts);
        response.NextCursor = accounts.Last().CreatedAt;
        response.HasMore = accounts.Count == req.PageSize;

        return TypedResults.Ok(response);
    }
}
