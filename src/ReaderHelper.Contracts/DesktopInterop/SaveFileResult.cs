namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class SaveFileResult
{
    public bool Cancelled { get; init; }

    public string? SavedPath { get; init; }
}
