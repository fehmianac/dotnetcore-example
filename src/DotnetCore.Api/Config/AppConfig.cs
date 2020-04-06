namespace DotnetCore.Api.Config
{
    public class AppConfig
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public CacheSettings CacheSettings { get; set; }
        public AuthorizationSettings AuthorizationSettings { get; set; }
        public SwaggerSettings SwaggerSettings { get; set; }
        public string Environment { get; set; }
    }

    public class CacheSettings
    {
        public string RedisConfiguration { get; set; }
        public string RedisInstanceName { get; set; }
    }

    public class AuthorizationSettings
    {
        public string JwtSecret { get; set; }
    }

    public class ConnectionStrings
    {
        public string UserDB { get; set; }
    }

    public class SwaggerSettings
    {
        public string DocName { get; set; }
        public string DocInfoTitle { get; set; }
        public string DocInfoVersion { get; set; }
    }
}