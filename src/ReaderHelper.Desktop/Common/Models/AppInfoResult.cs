namespace ReaderHelper.Desktop.Common.Models;

public sealed record AppInfoResult(
    string AppName,
    string Framework,
    string Os,
    string Version);
