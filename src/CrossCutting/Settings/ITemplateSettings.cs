namespace CrossCutting.Settings
{
    public interface ITemplateSettings
    {
        public const string Section = "Template";
        public string SqlServerConnectionString { get; }
        public string ImageApiUrl { get; }
        public string ImageApiKey { get; }
        public long MaxImageSizeMb { get; }
        public IEnumerable<string> AllowedImageExtensions { get; }
    }
}