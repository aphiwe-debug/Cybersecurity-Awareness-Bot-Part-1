namespace CybersecurityBot.Wpf.Models;

public record QuizQuestion(string Text, string[] Options, int CorrectIndex, string Explanation);