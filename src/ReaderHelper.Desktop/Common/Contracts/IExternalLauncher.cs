namespace ReaderHelper.Desktop.Common.Contracts;

public interface IExternalLauncher
{
    Models.OpenExternalResult OpenUri(Models.OpenExternalRequest request);
}
