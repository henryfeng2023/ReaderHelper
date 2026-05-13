using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Web.WebView2.Core;
using ReaderHelper.Desktop.Bridge;

namespace ReaderHelper.Desktop;

public partial class MainWindow : Window
{
    //private readonly DesktopBridgeDispatcher _dispatcher;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReaderHelper",
            "WebView2");

        var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
        await AppWebView.EnsureCoreWebView2Async(environment);

        AppWebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
        AppWebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
        AppWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        AppWebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

        //var webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        //AppWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
        //    "127.0.0.1:5173",
        //    "./www",
        //    CoreWebView2HostResourceAccessKind.Deny);

        // 4. 加载你的本地 Web 服务地址
        AppWebView.Source = new Uri("http://127.0.0.1:5500");
    }

    /// <summary>
    /// 异步检查本地开发服务器（DevServerUri）在短超时内是否可访问。
    /// </summary>
    /// <returns>
    /// 一个 <see cref="Task{Boolean}"/>，当开发服务器在指定超时时间内返回成功的 HTTP 状态码时为 <c>true</c>，否则为 <c>false</c>。
    /// </returns>
    /// <remarks>
    /// - 使用 800ms 的超时以避免在应用启动或 UI 线程上长时间阻塞。  
    /// - 仅请求响应头（<see cref="HttpCompletionOption.ResponseHeadersRead"/>），以便更快得知服务器是否在线。  
    /// - 捕获并吞噬所有异常（例如超时、连接拒绝、DNS 解析失败等）；任何异常都会被视为服务器不可用并返回 <c>false</c>。  
    /// - 该方法设计用于在调试（DEBUG）模式下决定是否将 WebView 指向本地开发服务器；在生产路径上不应依赖此方法。
    /// </remarks>
    private static async Task<bool> IsDevServerAvailableAsync()
    {
        try
        {
            var devServerUri = new Uri("http://127.0.0.1:5500");
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(800)
            };

            using var response = await client.GetAsync(devServerUri, HttpCompletionOption.ResponseHeadersRead);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        string msg = e.TryGetWebMessageAsString() ?? string.Empty;

        if (string.IsNullOrEmpty(msg))
        {
            Debug.WriteLine("Received empty message.");
            return;
        }
        else
        {
            Debug.WriteLine($"Received message: {msg}");
        }

        //BridgeResponse response;

        //try
        //{
        //    var request = JsonSerializer.Deserialize<BridgeRequest>(msg, BridgeJson.Options);
        //    if (request is null)
        //    {
        //        response = BridgeResponse.Error(string.Empty, "Invalid request payload.");
        //    }
        //    else
        //    {
        //        response = await _dispatcher.DispatchAsync(request);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    response = BridgeResponse.Error(string.Empty, ex.Message);
        //}

        //AppWebView.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(response, BridgeJson.Options));

        //if (response.Success && response.Data is CloseWindowResult)
        //{
        //    Dispatcher.BeginInvoke(Close);
        //}
    }
}
