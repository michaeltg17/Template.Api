namespace CrossCutting.Settings
{
    public record TemplateApiJwtSettings
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Key { get; set; }
        public int ExpirationMinutes { get; set; } = 4320;
        public string CookieName { get; set; } = "bulletproof_react_app_token";
    }
}
