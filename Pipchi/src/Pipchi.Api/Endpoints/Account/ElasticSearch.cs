using Microsoft.AspNetCore.Mvc;
using Pipchi.Api.Models.Account;
using Pipchi.Infrastructure.Elasticsearch;

[ApiController]
[Route("api/accounts")]
public class ElasticSearch : ControllerBase
{
    private readonly AccountSearchService _service;

    public ElasticSearch(AccountSearchService service)
    {
        _service = service;
    }

    [HttpPost("create-index")]
    public async Task<IActionResult> CreateIndex()
    {
        await _service.CreateIndexAsync();
        return Ok("Index created");
    }

    [HttpGet("search-accounts")]
    public async Task<IActionResult> SearchAccounts([FromQuery] AccountFilterRequest filter)
    {
        var accounts = await _service.SearchAsync(filter);
        return Ok(accounts);
    }
}
