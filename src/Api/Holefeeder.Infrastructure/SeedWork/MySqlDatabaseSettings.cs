using System.ComponentModel.DataAnnotations;

using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

public class MySqlDatabaseSettings
{
    private readonly string _basicConnectionString = null!;
    private readonly string _mySqlConnectionString = null!;

    [Required]
    public string ConnectionString
    {
        get => _mySqlConnectionString;
        init
        {
            _basicConnectionString = value;

            var builder = GetBuilder(true);
            builder.GuidFormat = MySqlGuidFormat.Binary16;
            builder.IgnoreCommandTransaction = true;

            _mySqlConnectionString = builder.ConnectionString;
        }
    }

    public MySqlConnectionStringBuilder GetBuilder(bool basic = false)
    {
        return new MySqlConnectionStringBuilder(basic ? _basicConnectionString : _mySqlConnectionString);
    }
}
