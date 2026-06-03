using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pipchi.Infrastructure.Data;
using Pipchi.SharedKernel;
using Quartz;
using System.Data;

namespace Pipchi.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IPublisher _publisher;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using var connection = _sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var domainEvent = JsonConvert.DeserializeObject<BaseDomainEvent>(
                    outboxMessage.Content,
                    jsonSerializerSettings)!;

                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while processing outbox message {outboxMessage.Id}");

                exception = ex;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = @"
            UPDATE OutboxMessages
            SET ProcessedOnUtc = @ProcessedOnUtc,
                Error = @Error
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = DateTime.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $"""
                   SELECT TOP ({_outboxOptions.BatchSize}) Id, Content
                   FROM OutboxMessages WITH (UPDLOCK, ROWLOCK, READPAST)
                   WHERE ProcessedOnUtc IS NULL
                   ORDER BY OccurredOnUtc
                   """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);

        return outboxMessages.ToList();
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
