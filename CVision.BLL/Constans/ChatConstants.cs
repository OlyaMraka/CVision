namespace CVision.BLL.Constans;

public static class ChatConstants
{
    public static readonly int MinMessageLength = 1;

    public static readonly int MaxMessageLength = 4000;

    public static readonly string MessageContentRequired
        = "Повідомлення не може бути порожнім!";

    public static readonly string MessageMaxLengthError
        = $"Довжина повідомлення не повинна перевищувати {MaxMessageLength} символів!";

    public static readonly string CannotMessageSelf
        = "Не можна надсилати повідомлення самому собі!";

    public static readonly string ReceiverNotFound
        = "Отримувача не знайдено!";

    public static readonly string SaveMessageError
        = "Помилка надсилання повідомлення! Спробуйте ще раз!";
}
