namespace ReaderHelper.Desktop.Common.Models;

public sealed record OpenFileResult(
    bool Success,
    string? FileName,
    IReadOnlyList<string> FileNames);
