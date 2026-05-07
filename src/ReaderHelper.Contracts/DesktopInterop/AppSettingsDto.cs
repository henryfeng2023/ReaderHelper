namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class AppSettingsDto
{
    public string Theme { get; init; } = "shell-sand";

    public string LastOpenedFolder { get; init; } = string.Empty;

    public string LastExportFileName { get; init; } = "readerhelper-export.txt";

    public string NotesDraft { get; init; } = string.Empty;
}
