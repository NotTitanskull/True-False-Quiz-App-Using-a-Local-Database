using SQLite;

namespace true_false_quiz;

public enum ManaColor
{
    White,
    Blue,
    Black,
    Red,
    Green
}

[Table("QuizQuestions")]
public sealed class QuizQuestion
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public string ImageFile { get; set; } = "";
    public string ArtCredit { get; set; } = "";
    public ManaColor Color { get; set; }
    public string Playstyle { get; set; } = "";
    public string Result { get; set; } = "";
    // A matching answer, rather than an objectively correct personality preference.
    public bool CorrectAnswer { get; set; }
}
