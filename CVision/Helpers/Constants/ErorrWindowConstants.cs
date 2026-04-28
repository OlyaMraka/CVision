namespace CVision.Helpers.Constants;

public static class ErorrWindowConstants
{
    public static readonly string WindowTitle = "Помилки";

    public static readonly string WindowAriaLabel = "Вікно помилок";

    public static readonly string Error400
        = "Здається, ви ввели щось не те. Перевірте дані та спробуйте ще раз.";

    public static readonly string Error401
        = "Ой! Схоже, термін вашої сесії вичерпано. Будь ласка, увійдіть знову.";

    public static readonly string Error403
        = "Вибачте, але у вас немає прав для перегляду цієї сторінки.";

    public static readonly string Error404
        = "Ми шукали всюди, але не змогли знайти таку сторінку в системі CVision.";

    public static readonly string Error408
        = "Сервер занадто довго чекав на відповідь. Перевірте з'єднання.";

    public static readonly string Error429
        = "Занадто багато запитів. Зачекайте хвилину і спробуйте ще раз.";

    public static readonly string Error500
        = "На сервері щось зламалося. Наші розробники вже отримали сповіщення.";

    public static readonly string UnknownError
        = "Сталася непередбачувана помилка. Спробуйте оновити сторінку.";
}
