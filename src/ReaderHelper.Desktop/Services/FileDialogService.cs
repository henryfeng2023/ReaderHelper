using System.Windows;
using Microsoft.Win32;
using ReaderHelper.Contracts.DesktopInterop;

namespace ReaderHelper.Desktop.Services;

internal static class FileDialogService
{
    public static async Task<FileDialogResult> SelectFilesAsync(FileDialogOptions? options)
    {
        return await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = options?.AllowMultiple ?? false
            };

            if (!string.IsNullOrWhiteSpace(options?.Title))
            {
                dialog.Title = options.Title;
            }

            if (!string.IsNullOrWhiteSpace(options?.Filter))
            {
                dialog.Filter = options.Filter;
            }

            var confirmed = dialog.ShowDialog() == true;

            return new FileDialogResult
            {
                Cancelled = !confirmed,
                SelectedPaths = confirmed ? dialog.FileNames : Array.Empty<string>()
            };
        });
    }
}
