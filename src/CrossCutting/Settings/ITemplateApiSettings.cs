namespace CrossCutting.Settings
{
    public interface ITemplateApiSettings
    {
        public const string Section = "TemplateApi";
        public string PostgreSqlConnectionString { get; }
        public Uri ImageApiUrl { get; }
        public string ImageApiKey { get; }
        public long MaxImageSizeMb { get; }
        public IEnumerable<string> AllowedImageExtensions { get; }
    }
}