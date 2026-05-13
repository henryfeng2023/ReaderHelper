namespace ReaderHelper.Desktop.Common.Contracts;

public interface IWindowControlService
{
    Models.WindowMinimizeResult Minimize();

    Models.CloseWindowResult RequestClose();
}
