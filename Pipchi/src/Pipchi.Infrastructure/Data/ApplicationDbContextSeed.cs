using Microsoft.Data.SqlClient;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.ValueObjects;
using Pipchi.Infrastructure.Elasticsearch;
using System.Data;

namespace Pipchi.Infrastructure.Data;

public class ApplicationDbContextSeed
{
    private static readonly string[] Currencies = { "USD", "EUR" };
    private static readonly int[] Leverages = { 10, 20, 50, 100, 200, 500 };

    public static async Task SeedAndIndexAsync(string connectionString, AccountSearchService searchService)
    {
        const int total = 1_000_000;
        const int batchSize = 10_000;

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        for (int i = 0; i < total / batchSize; i++)
        {
            var table = CreateTableSchema();
            var accounts = new List<Account>(batchSize);

            for (int j = 0; j < batchSize; j++)
            {
                var id = Guid.NewGuid();
                var currency = Currencies[Random.Shared.Next(Currencies.Length)];
                var amount = RandomAmount();
                var createdAt = DateTimeOffset.UtcNow;
                var leverage = Leverages[Random.Shared.Next(Leverages.Length)];

                table.Rows.Add(id, currency, amount, createdAt, DBNull.Value, leverage, 1L);

                accounts.Add(new Account
                    (
                        id,
                        new Money(amount, currency),
                        leverage
                    ));
            }

            using var bulk = new SqlBulkCopy(connection)
            {
                DestinationTableName = "Accounts",
                BatchSize = batchSize
            };

            await bulk.WriteToServerAsync(table);
            await searchService.BulkIndexAccountsAsync(accounts);

            Console.WriteLine($"Inserted and indexed {(i + 1) * batchSize:N0} rows...");
        }
    }

    private static DataTable CreateTableSchema()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));
        table.Columns.Add("Balance_Currency", typeof(string));
        table.Columns.Add("Balance_Amount", typeof(decimal));
        table.Columns.Add("CreatedAt", typeof(DateTimeOffset));
        table.Columns.Add("UpdatedAt", typeof(DateTimeOffset));
        table.Columns.Add("Leverage", typeof(int));
        table.Columns.Add("Version", typeof(long));
        return table;
    }

    private static decimal RandomAmount()
    {
        int baseNum = Random.Shared.Next(1, 1_000_001);
        int round = Random.Shared.Next(3) switch
        {
            0 => 1,
            1 => 10,
            2 => 100,
            _ => 1
        };
        return Math.Round(baseNum / (decimal)round) * round;
    }
}
