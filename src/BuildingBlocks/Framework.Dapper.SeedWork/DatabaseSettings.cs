using System;

using MySqlConnector;

namespace Framework.Dapper.SeedWork
{
    public class DatabaseSettings
    {
        private readonly string _connectionString;

        public string ConnectionString
        {
            get => _connectionString;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                BasicConnectionString = value;
                var builder = GetBuilder();
                builder.GuidFormat = MySqlGuidFormat.Binary16;
                builder.IgnoreCommandTransaction = true;
                _connectionString = builder.ConnectionString;
            }
        }

        public string BasicConnectionString { get; private set; }

        public MySqlConnectionStringBuilder GetBuilder() => new(BasicConnectionString);
    }
}
