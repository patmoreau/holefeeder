using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Dapper.SeedWork
{
    public interface IDbContext : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
    }
}
