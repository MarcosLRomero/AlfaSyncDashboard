using AlfaSyncDashboard.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AlfaSyncDashboard.Services;

public sealed class AppConfigService
{
    private readonly string _configPath;

    public AppConfigService()
    {
        _configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    }

    public AppSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        return configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        var wrapper = new Dictionary<string, AppSettings> { ["AppSettings"] = settings };
        var json = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }
}
