namespace Colpix.Data.Models
{
    public class JwtSettings
    {
        public string Key { get; set; } = "ChangeThisToAStrongKey-ReplaceInProduction";
        public string Issuer { get; set; } = "Colpix111111";
        public string Audience { get; set; } = "ColpixClients";
        // Default 5 minutes; configurable via appsettings
        public int TokenLifetimeMinutes { get; set; } = 5;
    }
}