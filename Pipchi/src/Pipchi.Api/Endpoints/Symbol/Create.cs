using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipchi.Contracts.DTOs;
using Pipchi.Contracts.Models.Symbol.Create;
using Pipchi.Core.Interfaces;
using Pipchi.SharedKernel.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.Endpoints.Symbol;

public class Create : Endpoint<CreateSymbolRequest, Results<Ok<CreateSymbolResponse>, Conflict<string>>>
{
    private readonly IRepository<Core.SyncedAggregates.Symbol> _repository;
    private readonly ISymbolUniquenessChecker _uniquenessChecker;
    private readonly ILogger<Create> _logger;
    private readonly IMapper _mapper;

    public Create(IRepository<Core.SyncedAggregates.Symbol> repository,
        ISymbolUniquenessChecker uniquenessChecker,
        ILogger<Create> logger,
        IMapper mapper)
    {
        _repository = repository;
        _uniquenessChecker = uniquenessChecker;
        _logger = logger;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post(CreateSymbolRequest.Route);
        AllowAnonymous();
        Description(x =>
            x.WithSummary("Creates a new Symbol")
            .WithDescription("Creates a new Symbol")
            .WithName("symbols.create")
            .WithTags("SymbolEndpoints"));
    }

    public override async Task<Results<Ok<CreateSymbolResponse>, Conflict<string>>> ExecuteAsync(CreateSymbolRequest request,
        CancellationToken cancellationToken)
    {
        var response = new CreateSymbolResponse(request.CorrelationId);

        if (!await _uniquenessChecker.IsNameUniqueAsync(request.Name, cancellationToken))
        {
            _logger.LogError("A symbol with the name '{SymbolName}' already exists.", request.Name);
            return TypedResults.Conflict($"A symbol with the name '{request.Name}' already exists.");
        }

        var symbol = new Core.SyncedAggregates.Symbol(request.Name, request.Digits, request.MinPrice,
            request.MaxPrice, request.MinVolume, request.MaxVolume);

        symbol = await _repository.AddAsync(symbol);

        _logger.LogInformation("Created new symbol with id {SymbolId} and name {SymbolName}", symbol.Id, symbol.Name);

        response.SymbolDto = _mapper.Map<SymbolDto>(symbol);

        return TypedResults.Ok(response);
    }
}
