using System.IO;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using ReaderHelper.Desktop.Services;

namespace ReaderHelper.Desktop;

public partial class App : Application
{
    private LocalApiProcessHost? _localApiProcessHost;

    internal static AppSettingsService SettingsService { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var cachePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReaderHelper",
            "CefCache");

        Directory.CreateDirectory(cachePath);

        var settings = new CefSettings
        {
            CachePath = cachePath,
            Locale = "zh-CN"
        };

        if (Cef.IsInitialized ?? false)
        {
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        _localApiProcessHost = new LocalApiProcessHost();
        _localApiProcessHost.TryStart();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _localApiProcessHost?.Dispose();

        if (Cef.IsInitialized ?? true)
        {
            Cef.Shutdown();
        }

        base.OnExit(e);
    }
}
