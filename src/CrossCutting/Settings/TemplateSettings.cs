namespace CrossCutting.Settings
{
    public class TemplateSettings : ITemplateSettings
    {
        public required long MaxImageSizeMb { get; set; }
        public required IEnumerable<string> AllowedImageExtensions { get; set; }
        public required string PostgreSqlConnectionString { get; set; }
        public required Uri ImageApiUrl { get; set; }
        public required string ImageApiKey { get; set; }
    }
}