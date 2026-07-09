namespace IntegrationTests.Settings
{
    public interface ITestSettings
    {
        public bool KeepAliveDatabase { get; }
        public bool EnableSqlLogging { get; }
    }
}
