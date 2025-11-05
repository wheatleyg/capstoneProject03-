namespace CapstoneBackend.Utilities;

public static class EnvironmentVariables
{
    private static readonly string Prefix = "EnvironmentVariables";
    
    public static readonly string TRUE_TEST_KEY = $"{Prefix}:Testing:TrueTestKey";
    public static readonly string FALSE_TEST_KEY = $"{Prefix}:Testing:FalseTestKey";
    public static readonly string MYSQL_CONNECTION_STRING = $"{Prefix}:Database:ConnectionString";
    public static readonly string TOKEN_KEY = $"{Prefix}:AuthTokenKey";
}