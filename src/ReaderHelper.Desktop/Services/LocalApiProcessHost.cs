using System.Diagnostics;
using System.IO;

namespace ReaderHelper.Desktop.Services;

internal sealed class LocalApiProcessHost : IDisposable
{
    private Process? _process;

    public bool TryStart()
    {
        if (_process is { HasExited: false })
        {
            return true;
        }

        var executablePath = DesktopEnvironmentService.ResolveLocalApiExecutablePath();
        if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
        {
            return false;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.Environment["READERHELPER_LOCAL_API_URL"] = DesktopEnvironmentService.ResolveLocalApiBaseUrl();

        _process = Process.Start(startInfo);
        return _process is not null;
    }

    public void Dispose()
    {
        if (_process is null)
        {
            return;
        }

        try
        {
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
                _process.WaitForExit(3000);
            }
        }
        catch
        {
        }
        finally
        {
            _process.Dispose();
            _process = null;
        }
    }
}
