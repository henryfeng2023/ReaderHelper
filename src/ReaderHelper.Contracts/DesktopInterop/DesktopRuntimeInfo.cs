namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class DesktopRuntimeInfo
{
    public required string AppName { get; init; }

    public required string AppVersion { get; init; }

    public required string HostKind { get; init; }

    public required string ChromiumMode { get; init; }

    public required string StartUrl { get; init; }

    public required string LocalApiBaseUrl { get; init; }

    public required string MachineName { get; init; }
}
