namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class FileDialogOptions
{
    public bool AllowMultiple { get; init; }

    public string? Title { get; init; }

    public string? Filter { get; init; }
}
