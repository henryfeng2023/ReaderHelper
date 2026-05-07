using System.Windows;

namespace ReaderHelper.Desktop.Services;

internal static class ClipboardService
{
    public static async Task<string> ReadTextAsync()
    {
        return await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;
        });
    }

    public static async Task<bool> WriteTextAsync(string text)
    {
        return await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Clipboard.SetText(text ?? string.Empty);
            return true;
        });
    }
}
