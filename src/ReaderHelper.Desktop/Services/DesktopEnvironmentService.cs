using System.IO;

namespace ReaderHelper.Desktop.Services;

internal static class DesktopEnvironmentService
{
    private const string DefaultLocalApiBaseUrl = "http://127.0.0.1:5057";
    private const string LocalApiExecutableVariable = "READERHELPER_LOCAL_API_EXE";

    public static string ResolveStartupUrl()
    {
        var configured = Environment.GetEnvironmentVariable("READERHELPER_START_URL");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var appBasePath = AppContext.BaseDirectory;
        var localIndexPath = Path.Combine(appBasePath, "www", "index.html");
        //return new Uri(localIndexPath).AbsoluteUri;
        return "http://localhost:5500/index.html";

    }

    public static string ResolveLocalApiBaseUrl()
    {
        var configured = Environment.GetEnvironmentVariable("READERHELPER_LOCAL_API_URL");
        return string.IsNullOrWhiteSpace(configured) ? DefaultLocalApiBaseUrl : configured;
    }

    public static string? ResolveLocalApiExecutablePath()
    {
        var configured = Environment.GetEnvironmentVariable(LocalApiExecutableVariable);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var appBasePath = AppContext.BaseDirectory;
        var exeName = OperatingSystem.IsWindows() ? "ReaderHelper.LocalApi.exe" : "ReaderHelper.LocalApi";
        var defaultPath = Path.Combine(appBasePath, exeName);

        return File.Exists(defaultPath) ? defaultPath : null;
    }
}
