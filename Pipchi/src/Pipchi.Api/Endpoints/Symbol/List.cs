using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Api.Models.Symbol;
using Pipchi.Api.Models.Symbol.List;
using Pipchi.Core.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.Endpoints.Symbol;

public class List : Endpoint<ListSymbolRequest, Results<Ok<ListSymbolResponse>, NotFound>>
{
    private readonly ISymbolCacheService _symbolCacheService;
    private readonly IMapper _mapper;

    public List(ISymbolCacheService symbolCacheService,
        IMapper mapper)
    {
        _symbolCacheService = symbolCacheService;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get(ListSymbolRequest.Route);
        AllowAnonymous();
        Description(d =>
            d.WithSummary("List Symbols")
             .WithDescription("List Symbols")
             .WithName("symbols.List")
             .WithTags("SymbolEndpoints"));
    }

    public override async Task<Results<Ok<ListSymbolResponse>, NotFound>> ExecuteAsync(
        ListSymbolRequest req,
        CancellationToken ct)
    {
        var response = new ListSymbolResponse(req.CorrelationId);

        var symbols = await _symbolCacheService.GetAllAsync(ct);

        if (symbols is null) return TypedResults.NotFound();

        response.Symbols = _mapper.Map<List<SymbolDto>>(symbols);

        return TypedResults.Ok(response);
    }
}
