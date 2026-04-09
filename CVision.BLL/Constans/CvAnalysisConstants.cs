namespace CVision.BLL.Constans;

public static class CvAnalysisConstants
{
    public static readonly int DaysAlive = 30;

    public static readonly string CvFileEmptyError
        = "Файл не може бути порожнім!";

    public static readonly string CvFileRequired
        = "Файл є обов’язковим!";

    public static readonly string CvSavingError
        = "Помилка збереження файлу";

    public static readonly string CvAnalysisSavingError
        = "Помилка збереження результатів аналізу";

    public static readonly string FileNameRequired
        = "Назва файлу є обов’язковою!";

    public static readonly string IncorrectFormatError =
        "Недопустимий формат файлу! Дозволено: PDF, DOCX, JPG, JPEG, PNG";

    public static readonly string InvalidUserData
        = "Некоректний користувач!";

    public static readonly string DbDeleteError
        = "Помилка при видаленні аналізу";

    public static string CvSizeError(int size)
    {
        return $"Розмір файлу не повинен перевищувати {size} MB!";
    }
}