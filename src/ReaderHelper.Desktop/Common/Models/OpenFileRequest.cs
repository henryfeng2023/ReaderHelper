namespace ReaderHelper.Desktop.Common.Models;

public sealed record OpenFileRequest(
    string? Filter,
    bool Multiselect);
