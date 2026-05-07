namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class FileDialogResult
{
    public bool Cancelled { get; init; }

    public IReadOnlyList<string> SelectedPaths { get; init; } = Array.Empty<string>();
}
