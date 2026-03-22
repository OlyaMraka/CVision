namespace CVision.BLL.Constans;

public static class ServiceConstants
{
    public static readonly string EmailConfigurationError
        = "Email settings are missing in configuration.";

    public static string ExtensionNotSupported(string extension)
    {
        return $"Формат файлу {extension} не підтримується системою.";
    }
}