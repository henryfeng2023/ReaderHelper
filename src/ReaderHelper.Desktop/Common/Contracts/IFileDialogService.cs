
namespace ReaderHelper.Desktop.Common.Contracts;

public interface IFileDialogService
{
    Models.OpenFileResult OpenFile(Models.OpenFileRequest request);
}
