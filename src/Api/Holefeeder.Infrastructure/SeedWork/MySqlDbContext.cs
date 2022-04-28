using System.Data;
using System.Data.SqlClient;

using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

public class MySqlDbContext : IDbContext
{
    private readonly MySqlDatabaseSettings _settings;

    private MySqlConnection? _connection;

    private bool _isDisposed;
    private MySqlTransaction? _transaction;

    public MySqlDbContext(MySqlDatabaseSettings settings)
    {
        _settings = settings;
    }

    public IDbConnection Connection
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            return _connection ??= CreateConnection(_settings.ConnectionString);
        }
    }

    public IDbTransaction Transaction
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (_transaction is not null)
            {
                return _transaction;
            }

            _connection ??= CreateConnection(_settings.ConnectionString);
            _transaction = _connection.BeginTransaction();

            return _transaction;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        if (_transaction is null)
        {
            throw new InvalidOperationException("No transaction created");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch (SqlException)
        {
            await _transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private static MySqlConnection CreateConnection(string connectionString)
    {
        var connection = new MySqlConnection(connectionString);
        connection.Open();

        return connection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        _isDisposed = true;
    }
}
