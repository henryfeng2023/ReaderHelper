namespace ReaderHelper.LocalApi.Services;

internal static class LocalApiEnvironment
{
    private const string DefaultListenUrl = "http://127.0.0.1:5057";

    public static string ResolveListenUrl()
    {
        var configured = Environment.GetEnvironmentVariable("READERHELPER_LOCAL_API_URL");
        return string.IsNullOrWhiteSpace(configured) ? DefaultListenUrl : configured;
    }
}
