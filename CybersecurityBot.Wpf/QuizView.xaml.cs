using CybersecurityBot.Wpf.Models;
using CybersecurityBot.Wpf.Services;
using Google.Protobuf.WellKnownTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityBot.Wpf;

public partial class QuizView : UserControl
{
    private readonly Button[] _options;
    private ActivityLog? _log;
    private List<QuizQuestion> _questions = new();
    private int _index;
    private int _score;
    private bool _answered;

    public QuizView()
    {
        InitializeComponent();
        _options = new[] { Option0, Option1, Option2, Option3 };
    }

    public void Initialize(ActivityLog log) => _log = log;

    /// <summary>Starts a fresh quiz. Also called from the chat when the user says "start quiz".</summary>
    public void StartQuiz()
    {
        _questions = QuizService.NewQuiz();
        _index = 0;
        _score = 0;
        _log?.Add($"Quiz started ({_questions.Count} questions)");

        StartPanel.Visibility = Visibility.Collapsed;
        ResultPanel.Visibility = Visibility.Collapsed;
        QuestionPanel.Visibility = Visibility.Visible;
        ShowQuestion();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e) => StartQuiz();

    private void ShowQuestion()
    {
        _answered = false;
        QuizQuestion q = _questions[_index];

        ProgressText.Text = $"Question {_index + 1} of {_questions.Count}   |   Score: {_score}";
        QuestionText.Text = q.Text;
        FeedbackBox.Visibility = Visibility.Collapsed;
        NextButton.Visibility = Visibility.Collapsed;

        for (int i = 0; i < _options.Length; i++)
        {
            Button button = _options[i];
            if (i < q.Options.Length)
            {
                button.Visibility = Visibility.Visible;
                button.Background = Hex("#21262D");
                button.Content = new TextBlock
                {
                    Text = $"{(char)('A' + i)})  {q.Options[i]}",
                    TextWrapping = TextWrapping.Wrap
                };
            }
            else
            {
                button.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void Option_Click(object sender, RoutedEventArgs e)
    {
        if (_answered || _questions.Count == 0) return;
        _answered = true;

        int chosen = int.Parse((string)((Button)sender).Tag);
        QuizQuestion q = _questions[_index];
        bool correct = chosen == q.CorrectIndex;
        if (correct) _score++;

        _options[q.CorrectIndex].Background = Hex("#238636");
        if (!correct) _options[chosen].Background = Hex("#DA3633");

        FeedbackText.Text = (correct ? "✔ Correct! " : "✘ Not quite. ") + q.Explanation;
        FeedbackBox.Background = Hex(correct ? "#1B4332" : "#5A1D1D");
        FeedbackBox.Visibility = Visibility.Visible;

        ProgressText.Text = $"Question {_index + 1} of {_questions.Count}   |   Score: {_score}";
        NextButton.Content = _index == _questions.Count - 1 ? "See my score" : "Next question";
        NextButton.Visibility = Visibility.Visible;
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        _index++;
        if (_index >= _questions.Count) ShowResult();
        else ShowQuestion();
    }

    private void ShowResult()
    {
        QuestionPanel.Visibility = Visibility.Collapsed;
        ResultPanel.Visibility = Visibility.Visible;
        ScoreText.Text = $"{_score} / {_questions.Count}";
        ResultMessage.Text = QuizService.Feedback(_score, _questions.Count);
        _log?.Add($"Quiz completed: scored {_score}/{_questions.Count}");
    }

    private static Brush Hex(string hex) => (Brush)new BrushConverter().ConvertFrom(hex)!;
}