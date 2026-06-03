using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pipchi.Api.Models.Account;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Infrastructure.Elasticsearch;

public class AccountSearchService
{
    private readonly ElasticsearchClient _client;
    private readonly IConfiguration _configuration;
    private const string IndexName = "accounts";

    public AccountSearchService(ElasticsearchClient client,
        IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task CreateIndexAsync()
    {
        var exists = await _client.Indices.ExistsAsync(ElasticIndexes.Accounts);

        if (exists.Exists)
            return;

        await _client.Indices.CreateAsync<AccountDocument>(ElasticIndexes.Accounts, c => c
            .Settings(s => s
                .RefreshInterval("-1")
                .NumberOfReplicas("0")
            )
            .Mappings(m => m
                .Properties(p => p
                    .Keyword(k => k.Id)
                    .Keyword(k => k.BalanceCurrency)
                    .DoubleNumber(d => d.BalanceAmount)
                    .Date(d => d.CreatedAt)
                    .Date(d => d.UpdatedAt)
                    .IntegerNumber(i => i.Leverage)
                )
            )
        );
    }

    public async Task BulkIndexAccountsAsync(IEnumerable<Account> accounts)
    {
        var docs = accounts.Select(account => new AccountDocument
        {
            Id = account.Id,
            BalanceAmount = account.Balance.Amount,
            BalanceCurrency = account.Balance.Currency,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt,
            Leverage = account.Leverage
        });

        await _client.BulkAsync(b => b
            .Index(IndexName)
            .IndexMany(docs)
        );
    }

    public async Task<List<Account>> SearchAsync(AccountFilterRequest filter)
    {
        var mustClauses = new List<Action<QueryDescriptor<Account>>>();

        if (!string.IsNullOrEmpty(filter.Currency))
            mustClauses.Add(q => q
                .Term(t => t
                    .Field(f => f.Balance.Currency.Suffix("keyword"))
                    .Value(filter.Currency)
                )
            );

        if (filter.Leverage.HasValue)
            mustClauses.Add(q => q
                .Term(t => t
                    .Field(f => f.Leverage)
                    .Value(filter.Leverage.Value)
                )
            );

        if (filter.MinBalance.HasValue || filter.MaxBalance.HasValue)
            mustClauses.Add(q => q
                .Range(r => r
                    .NumberRange(n =>
                    {
                        n.Field(f => f.Balance.Amount);

                        if (filter.MinBalance.HasValue)
                            n.Gte((double)filter.MinBalance.Value);

                        if (filter.MaxBalance.HasValue)
                            n.Lte((double)filter.MaxBalance.Value);
                    })
                )
            );

        if (filter.CreatedFrom.HasValue || filter.CreatedTo.HasValue)
            mustClauses.Add(q => q
                .Range(r => r
                    .DateRange(n =>
                    {
                        n.Field(f => f.CreatedAt);

                        if (filter.CreatedFrom.HasValue)
                            n.Gte(filter.CreatedFrom.Value.UtcDateTime);

                        if (filter.CreatedTo.HasValue)
                            n.Lte(filter.CreatedTo.Value.UtcDateTime);
                    })
                )
            );

        var response = await _client.SearchAsync<Account>(s => s
            .Indices(ElasticIndexes.Accounts)
            .From((filter.Page - 1) * filter.PageSize)
            .Size(filter.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustClauses.ToArray())
                )
            )
        );

        return response.Documents.ToList();
    }
}
