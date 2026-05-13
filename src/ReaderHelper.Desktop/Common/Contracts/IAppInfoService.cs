namespace ReaderHelper.Desktop.Common.Contracts;

public interface IAppInfoService
{
    Models.AppInfoResult GetAppInfo();

    Models.PingResult Ping();
}
