namespace CrossCutting.Settings
{
    public interface ITemplateSettings
    {
        public string ApiUrl { get; }
        public string ImagesStoragePath { get; }
        public string ImagesRequestPath { get; }
        public string SqlServerConnectionString { get; }
        public long MaxImageSizeMb { get; }
        public IEnumerable<string> AllowedImageExtensions { get; }
    }
}