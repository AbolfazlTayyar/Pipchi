using Microsoft.Data.SqlClient;
using System.Data;

namespace Pipchi.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();

        return connection;
    }
}