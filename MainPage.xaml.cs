namespace true_false_quiz;

public partial class MainPage : ContentPage
{
    private QuizQuestion[] questions = [];
    private readonly QuizDatabase database = new(
        Path.Combine(FileSystem.AppDataDirectory, "quiz.db3"));
    private bool isLoading;
    private int score;

    private readonly List<bool> userAnswers = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (isLoading || questions.Length > 0)
            return;

        isLoading = true;
        QuestionCounterLabel.Text = "Loading questions…";
        try
        {
            questions = await database.LoadQuestionsAsync();
            ShowQuestion();
            QuestionCard.IsVisible = true;
            AnswerButtons.IsVisible = true;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            QuestionCounterLabel.Text = "Unable to load questions. Reopen this page to try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OnArtworkSizeChanged(object? sender, EventArgs e)
    {
        // The five art crops are approximately 1.37 times as wide as they are tall.
        if (QuestionImage.Width > 0)
            QuestionImage.HeightRequest = Math.Min(QuestionImage.Width / 1.37, 480);
    }

    private void ShowQuestion()
    {
        int index = userAnswers.Count;

        QuestionLabel.Text = questions[index].Text;
        QuestionImage.Source = questions[index].ImageFile;
        ArtCredit.Text = questions[index].ArtCredit;
        QuestionCounterLabel.Text = $"Question {index + 1} of {questions.Length}";
        QuizProgress.Progress = (double)index / questions.Length;
    }

    private void OnAnswerSwiped(object? sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Right)
            SubmitAnswer(true);
        else if (e.Direction == SwipeDirection.Left)
            SubmitAnswer(false);
    }

    private void OnTrueClicked(object? sender, EventArgs e) => SubmitAnswer(true);
    private void OnFalseClicked(object? sender, EventArgs e) => SubmitAnswer(false);

    private void SubmitAnswer(bool answer)
    {
        if (isLoading || userAnswers.Count >= questions.Length)
            return;

        if (answer == questions[userAnswers.Count].CorrectAnswer)
            score++;
        userAnswers.Add(answer);

        if (userAnswers.Count == questions.Length)
            ShowResults();
        else
            ShowQuestion();
    }

    private void ShowResults()
    {
        AnswerButtons.IsVisible = false;
        QuestionCard.GestureRecognizers.Clear();
        QuestionCounterLabel.Text = "Quiz complete";
        QuizProgress.Progress = 1;

        QuestionImage.IsVisible = false;
        ArtCredit.IsVisible = false;
        Instructions.IsVisible = false;

        var matches = questions.Where((question, index) => userAnswers[index] == question.CorrectAnswer).ToArray();
        if (matches.Length == 1)
        {
            ShowColor(matches[0]);
            return;
        }

        QuestionCounterLabel.Text = $"{matches.Length} of {questions.Length} colors matched";
        QuestionLabel.Text = matches.Length == 0
            ? "Which approach would you be most willing to try?"
            : "Several statements fit you. Which approach feels most like yours?";

        foreach (var candidate in matches.Length == 0 ? questions : matches)
        {
            var button = new Button { Text = candidate.Playstyle, MinimumHeightRequest = 48 };
            button.Clicked += (_, _) => ShowColor(candidate);
            TieChoices.Children.Add(button);
        }

        TieChoices.IsVisible = true;
    }

    private void ShowColor(QuizQuestion question)
    {
        TieChoices.IsVisible = false;
        QuestionCounterLabel.Text = "Quiz complete";
        QuestionLabel.Text = $"{question.Color}\n{question.Result}\n{score} of {questions.Length} colors matched.";
    }
}