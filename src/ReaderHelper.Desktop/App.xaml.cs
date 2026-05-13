using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using ReaderHelper.Desktop.Services;

namespace ReaderHelper.Desktop;

public partial class App : Application
{
    private LocalApiProcessHost? _localApiProcessHost;

    internal static AppSettingsService SettingsService { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _localApiProcessHost = new LocalApiProcessHost();
        _localApiProcessHost.TryStart();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _localApiProcessHost?.Dispose();

        base.OnExit(e);
    }
}
