namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class SaveFileRequest
{
    public string? Title { get; init; }

    public string? SuggestedFileName { get; init; }

    public string? Filter { get; init; }

    public required string Content { get; init; }
}
