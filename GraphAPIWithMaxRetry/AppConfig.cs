using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;


public static class AppConfig
{
    private static IConfiguration Configuration { get; }

    static AppConfig()
    {
        var path = System.IO.Directory.GetCurrentDirectory();
        // Build configuration from appsettings.json
        Configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public static string AADClientId => Configuration["AADClientId"];
    public static string TenantId => Configuration["TenantId"];
    public static string ClientSigningCertificatePath => Configuration["InsightAppClientCert"];
}
