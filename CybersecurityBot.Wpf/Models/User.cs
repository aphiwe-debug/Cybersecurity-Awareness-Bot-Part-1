using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBot.Wpf.Models;

public class User
{
    public string Name { get; set; } = string.Empty;
    public string FavouriteTopic { get; set; } = string.Empty;
    public string LastTopic { get; set; } = string.Empty;
    public int QuestionsAsked { get; set; }
}