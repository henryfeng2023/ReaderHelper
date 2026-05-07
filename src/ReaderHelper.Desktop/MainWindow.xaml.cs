using System.Diagnostics;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using ReaderHelper.Desktop.Bridge;
using ReaderHelper.Desktop.Services;

namespace ReaderHelper.Desktop;

public partial class MainWindow : Window
{
    private ChromiumWebBrowser? _browser;

    public MainWindow()
    {
        InitializeComponent();
        InitializeBrowser();
    }

    private void InitializeBrowser()
    {
        var startUrl = DesktopEnvironmentService.ResolveStartupUrl();
        var localApiBaseUrl = DesktopEnvironmentService.ResolveLocalApiBaseUrl();
        CefSharpSettings.ConcurrentTaskExecution = true;
        _browser = new ChromiumWebBrowser
        {
            Address = "about:blank"
        };

        _browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;

        _browser.JavascriptObjectRepository.Register(
            name: "desktop",
            objectToBind: new DesktopBridge(startUrl, localApiBaseUrl),
            //isAsyncEnabled: true,
            options: BindingOptions.DefaultBinder);

        BrowserHost.Children.Add(_browser);
        _browser.Address = startUrl;
    }

    protected override void OnClosed(EventArgs e)
    {
        _browser?.Dispose();
        base.OnClosed(e);
    }
}
