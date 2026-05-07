using System.IO;
using System.Text.Json;
using ReaderHelper.Contracts.DesktopInterop;

namespace ReaderHelper.Desktop.Services;

internal sealed class AppSettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _settingsPath;

    public AppSettingsService()
    {
        var settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReaderHelper");

        Directory.CreateDirectory(settingsDirectory);
        _settingsPath = Path.Combine(settingsDirectory, "appsettings.json");
    }

    public async Task<AppSettingsDto> ReadAsync()
    {
        await _lock.WaitAsync();

        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new AppSettingsDto();
            }

            await using var stream = File.OpenRead(_settingsPath);
            var settings = await JsonSerializer.DeserializeAsync<AppSettingsDto>(stream, JsonOptions);
            return settings ?? new AppSettingsDto();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<AppSettingsDto> WriteAsync(AppSettingsDto settings)
    {
        await _lock.WaitAsync();

        try
        {
            await using var stream = File.Create(_settingsPath);
            await JsonSerializer.SerializeAsync(stream, settings, JsonOptions);
            await stream.FlushAsync();
            return settings;
        }
        finally
        {
            _lock.Release();
        }
    }
}
