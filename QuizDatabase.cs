using SQLite;

namespace true_false_quiz;

internal sealed class QuizDatabase
{
    private readonly string databasePath;

    public QuizDatabase(string databasePath)
    {
        this.databasePath = databasePath;
    }

    public Task<QuizQuestion[]> LoadQuestionsAsync() => Task.Run(() =>
    {
        using var database = new SQLiteConnection(databasePath);
        database.CreateTable<QuizQuestion>();
        database.RunInTransaction(() =>
        {
            if (database.Table<QuizQuestion>().Count() == 0)
                database.InsertAll(SampleQuestions());
        });
        return database.Table<QuizQuestion>().OrderBy(question => question.Id).ToArray();
    });

    private static QuizQuestion[] SampleQuestions() =>
    [
        new() { Id = 1, Text = "I build a team of creatures that can protect each other.", ImageFile = "banners_art.jpg",
            ArtCredit = "Raise the Alarm · Zoltan Boros", Color = ManaColor.White, Playstyle = "Protect the team",
            Result = "You build a board where every ally helps the next.", CorrectAnswer = true },
        new() { Id = 2, Text = "I like drawing extra cards so I can find the right answer.", ImageFile = "spellbook_art.jpg",
            ArtCredit = "Divination · Willian Murai", Color = ManaColor.Blue, Playstyle = "Gather information",
            Result = "You keep your options open and find answers through knowledge.", CorrectAnswer = true },
        new() { Id = 3, Text = "I am willing to spend life to gain an advantage.", ImageFile = "blood_art.jpg",
            ArtCredit = "Sign in Blood · Howard Lyon", Color = ManaColor.Black, Playstyle = "Trade life for power",
            Result = "You see life as a resource and know when an advantage is worth its price.", CorrectAnswer = true },
        new() { Id = 4, Text = "When I see an opening, I want to strike immediately.", ImageFile = "lightning_art.jpg",
            ArtCredit = "Lightning Strike · Adam Paquette", Color = ManaColor.Red, Playstyle = "Seize the moment",
            Result = "You trust your instincts and make opponents react to you.", CorrectAnswer = true },
        new() { Id = 5, Text = "I enjoy growing my mana supply to cast enormous creatures.", ImageFile = "growth_art.jpg",
            ArtCredit = "Rampant Growth · Steven Belledin", Color = ManaColor.Green, Playstyle = "Grow my resources",
            Result = "You cultivate your lands and let your creatures grow into the biggest threats.", CorrectAnswer = true }
    ];
}
