namespace CVision.BLL.Constans;

public static class UserConstants
{
    public static readonly int MaxEmailLength = 40;
    public static readonly int MinEmailLength = 4;
    public static readonly int MaxUserNameLength = 40;
    public static readonly int MinUserNameLength = 4;

    public static readonly string EmailRequiredErrorMessage
        = "Електронна пошта є обов’язковою!";

    public static readonly string MaxEmailLengthErrorMessage
        = $"Електронна пошта повинна містити не більше {MaxEmailLength} символів!";

    public static readonly string MinEmailLengthErrorMessage
        = $"Електронна пошта повинна містити щонайменше {MinEmailLength} символи!";

    public static readonly string UserNameRequiredErrorMessage
        = "Ім’я користувача є обов’язковим!";

    public static readonly string MaxUserNameErrorMessage
        = $"Ім’я користувача повинно містити не більше {MaxUserNameLength} символів!";

    public static readonly string MinUserNameErrorMessage
        = $"Ім’я користувача повинно містити щонайменше {MinUserNameLength} символи!";

    public static readonly string PasswordRequiredErrorMessage
        = "Пароль є обов’язковим!";

    public static readonly string UserLogInError
        = "Невірний логін або пароль!";

    public static readonly string UserNotFound
        = "Користувача не знайдено!";

    public static readonly string EmailConfirmationError
        = "Не вдалося підтвердити електронну пошту!";
}