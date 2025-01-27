using System.Diagnostics.CodeAnalysis;

namespace Holefeeder.Application.Authorization;

[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public static class Configuration
{
    public static class Schemes
    {
        public const string Admin = nameof(Admin);
        public const string ReadUser = nameof(ReadUser);
        public const string WriteUser = nameof(WriteUser);
    }

    public static class Scopes
    {
        public const string ReadUser = "read:user";
        public const string WriteUser = "write:user";
    }

    public static class Policies
    {
        public const string ReadUser = nameof(ReadUser);
        public const string WriteUser = nameof(WriteUser);
    }

    public static class Roles
    {
        public const string Admin = "admin";
    }
}
