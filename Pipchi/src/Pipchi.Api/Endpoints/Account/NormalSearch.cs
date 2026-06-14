using Microsoft.AspNetCore.Mvc;
using Pipchi.Api.Models.Account;
using Pipchi.Core.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.Endpoints.Account;
public class NormalSearch : Controller
{
    private readonly IAccountRepository _repository;
    private readonly IMapper _mapper;

    public NormalSearch(
        IAccountRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("search-accounts")]
    public async Task<IActionResult> SearchAccounts(
        [FromQuery] AccountFilterRequest filter,
        CancellationToken ct = default)
    {
        var accounts = await _repository.SearchAsync(
            filter.Currency,
            filter.MinBalance,
            filter.MaxBalance,
            filter.Leverage,
            filter.CreatedFrom,
            filter.CreatedTo,
            filter.Page,
            filter.PageSize,
            ct);

        var result = _mapper.Map<List<AccountDto>>(accounts);

        return Ok(result);
    }
}
