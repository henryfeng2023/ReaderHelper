using System.IO;
using System.Windows;
using Microsoft.Win32;
using ReaderHelper.Contracts.DesktopInterop;

namespace ReaderHelper.Desktop.Services;

internal static class SaveFileService
{
    public static async Task<SaveFileResult> SaveTextAsync(SaveFileRequest request)
    {
        return await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var dialog = new SaveFileDialog
            {
                FileName = string.IsNullOrWhiteSpace(request.SuggestedFileName)
                    ? "readerhelper-export.txt"
                    : request.SuggestedFileName
            };

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                dialog.Title = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                dialog.Filter = request.Filter;
            }

            var confirmed = dialog.ShowDialog() == true;
            if (!confirmed)
            {
                return new SaveFileResult
                {
                    Cancelled = true
                };
            }

            await File.WriteAllTextAsync(dialog.FileName, request.Content ?? string.Empty);

            return new SaveFileResult
            {
                Cancelled = false,
                SavedPath = dialog.FileName
            };
        }).Task.Unwrap();
    }
}
