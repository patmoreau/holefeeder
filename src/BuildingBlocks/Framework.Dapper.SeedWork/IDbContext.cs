using System;
using System.Data;

namespace Framework.Dapper.SeedWork;

public interface IDbContext : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
}
