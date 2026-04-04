namespace CVision.BLL.Constans;

public static class CommentsConstants
{
    public static readonly int MinCommentLength = 3;

    public static readonly int MaxCommentLength = 1000;

    public static readonly string CommentContentRequired
        = "Коментар не можу бути порожнім!";

    public static readonly string CommentMaxLenghtError
        = $"Довжина коментаря не повинна перевищувати {MaxCommentLength} символів!";

    public static readonly string CommentMinLenghtError
        = $"Довжина коментаря не повинна перевищувати {MinCommentLength} символів!";

    public static readonly string SaveCommentError
        = "Помилка збереження коментаря! Спробуйте ще раз!";

    public static readonly string CommentNotFound
        = "Коментар не знайдено!";

    public static readonly string DeleteCommentError
        = "Помилка видалення коментаря! Спробуйте ще раз!";
}