namespace CrossCutting.Settings
{
    public class TemplateSettings : ITemplateSettings
    {
        public const string Section = "Api";
        public required string ApiUrl { get; set; }
        public required string ImagesStoragePath { get; set; }
        public required string ImagesRequestPath { get; set; }
        public required long MaxImageSizeMb { get; set; }
        public required IEnumerable<string> AllowedImageExtensions { get; set; }
        public required string SqlServerConnectionString { get; set; }
    }
}