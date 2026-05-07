using System.Diagnostics;
using ReaderHelper.Contracts.DesktopInterop;
using ReaderHelper.Desktop.Services;

namespace ReaderHelper.Desktop.Bridge;

public sealed class DesktopBridge
{
    private readonly string _startUrl;
    private readonly string _localApiBaseUrl;

    public DesktopBridge(string startUrl, string localApiBaseUrl)
    {
        _startUrl = startUrl;
        _localApiBaseUrl = localApiBaseUrl;
    }

    public Task<string> PingAsync(string message)
    {
        return Task.FromResult($"pong:{message}");
    }

    public Task<DesktopRuntimeInfo> GetRuntimeInfoAsync()
    {
        var version = typeof(DesktopBridge).Assembly.GetName().Version?.ToString() ?? "0.0.0";

        return Task.FromResult(new DesktopRuntimeInfo
        {
            AppName = "ReaderHelper",
            AppVersion = version,
            HostKind = "desktop-shell",
            ChromiumMode = "cefsharp",
            StartUrl = _startUrl,
            LocalApiBaseUrl = _localApiBaseUrl,
            MachineName = Environment.MachineName
        });
    }

    public Task<DesktopOperationResult> OpenExternalAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return Task.FromResult(DesktopOperationResult.Fail("Invalid URL."));
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            return Task.FromResult(DesktopOperationResult.Ok());
        }
        catch (Exception ex)
        {
            return Task.FromResult(DesktopOperationResult.Fail(ex.Message));
        }
    }

    public Task<FileDialogResult> SelectFilesAsync(FileDialogOptions? options)
    {
        return FileDialogService.SelectFilesAsync(options);
    }

    public Task<string> ReadClipboardTextAsync()
    {
        return ClipboardService.ReadTextAsync();
    }

    public Task<bool> WriteClipboardTextAsync(string text)
    {
        return ClipboardService.WriteTextAsync(text);
    }

    public Task<SaveFileResult> SaveTextFileAsync(SaveFileRequest request)
    {
        return SaveFileService.SaveTextAsync(request);
    }

    public Task<AppSettingsDto> GetAppSettingsAsync()
    {
        return App.SettingsService.ReadAsync();
    }

    public Task<AppSettingsDto> SaveAppSettingsAsync(AppSettingsDto settings)
    {
        return App.SettingsService.WriteAsync(settings);
    }
}
