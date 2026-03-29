namespace CVision.BLL.Constans;

public static class PublicationsConstants
{
    public static readonly int MinTitleLenght = 3;

    public static readonly int MaxTitleLenght = 30;

    public static readonly int MinDescriptionLenght = 10;

    public static readonly int MaxDescriptionLenght = 1000;

    public static readonly string FileEmptyError
        = "Файл не може бути порожнім!";

    public static readonly string FileRequired
        = "Файл є обов’язковим!";

    public static readonly string TitleRequired
        = "Назва допису обов'язкова!";

    public static readonly string DescriptionRequired
        = "Опис допису обов'язковий!";

    public static readonly string MinTitleLenghtError
        = $"Назва повинна містити не менше {MinTitleLenght} символів!";

    public static readonly string MaxTitleLenghtError
        = $"Назва повинна містити не більше {MaxTitleLenght} символів!";

    public static readonly string MinDescriptionLenghtError
        = $"Опис допису повинен містити не менше  {MinDescriptionLenght} символів!";

    public static readonly string MaxDescriptionLenghtError
        = $"Опис допису повинен містити не більше {MaxDescriptionLenght} символів!";

    public static readonly string FileNameRequired
        = "Назва файлу є обов’язковою!";

    public static readonly string IncorrectFormatError =
        "Недопустимий формат файлу! Дозволено: PDF, DOCX, JPG, JPEG, PNG";

    public static string FileSizeError(int size)
    {
        return $"Розмір файлу не повинен перевищувати {size} MB!";
    }
}