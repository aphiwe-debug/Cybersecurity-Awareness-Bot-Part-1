using System.Media;

namespace CybersecurityChatbot
{

    // MODEL: User
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public int QuestionsAsked { get; set; } = 0;
    }