# Cybersecurity Awareness Bot

A C# console chatbot that educates users on basic cybersecurity practices — passwords, phishing, safe browsing, and malware — through a simple, styled command-line conversation.

## Features

- **Voice greeting** — plays a `.wav` welcome sound on launch (Windows only, via `System.Media.SoundPlayer`)
- **ASCII art banner** — displays a logo when the app starts
- **Styled console UI** — colour-coded bot/user messages, dividers, headers, and a typewriter text effect
- **Personalized conversation** — asks for the user's name and uses it throughout the session
- **Keyword-based Q&A engine** — recognizes topics like passwords, phishing, safe browsing, and malware, and responds with relevant tips
- **Input validation** — handles empty/whitespace input and unrecognized questions gracefully
- **Exit commands** — type `exit`, `quit`, or `bye` at any time to end the session

## Project Structure

| Component | Responsibility |
|---|---|
| `User` | Stores the user's name and a running count of questions asked |
| `ConsoleUI` | Static helper for colours, dividers, headers, typing effect, and prompts |
| `AsciiArtService` | Displays the ASCII logo on startup |
| `AudioService` | Plays the welcome `.wav` file, with graceful fallback if missing/unsupported |
| `ResponseService` | Core chatbot logic — validates input and maps keywords to responses |
| `Program` | Entry point — wires everything together and runs the main conversation loop |

## Requirements

- **.NET SDK** (6.0 or later recommended)
- **Windows OS** for the voice greeting feature (`SoundPlayer` is Windows-only; the app still runs on other platforms, just without audio)
- A `welcome.wav` audio file placed in the application's base directory (next to the executable)

## Setup & Running

1. Clone or download the project files.
2. Place a `welcome.wav` file in the project's output/base directory (same folder as the compiled `.exe`, or the working directory when run via `dotnet run`).
3. Build and run:
   ```bash
   dotnet build
   dotnet run
   ```
4. Follow the prompts: enter your name, then ask about cybersecurity topics.

## Usage

Once running, you can ask things like:

- `What's a good password?`
- `How do I spot a phishing email?`
- `Is this link safe to click?`
- `What is malware?`
- `What can I ask you?`

Type `exit`, `quit`, or `bye` to end the conversation.

## Notes

- The chatbot matches responses using simple keyword detection (`Contains`), not natural language processing — phrasing matters.
- If a message doesn't match any known topic, the bot asks the user to rephrase.
- Audio playback failures (missing file, unsupported platform) are caught and logged as warnings without crashing the app.

## Possible Improvements

- Add more topics (e.g. two-factor authentication, social engineering, ransomware)
- Track and summarize `QuestionsAsked` at session end
- Replace keyword matching with a more flexible NLP/intent-matching approach
- Cross-platform audio support

Part 2: WPF Chatbot
Requirements
Windows 10 or 11 (the voice greeting uses System.Media.SoundPlayer, which is Windows only)
.NET 8 SDK
Visual Studio 2022 with the .NET desktop development workload
How to run
Clone the repository:
bash
   git clone <your repository URL>
Open CybersecurityBot.sln in Visual Studio.
In Solution Explorer, right-click CybersecurityBot.Wpf and choose Set as Startup Project.
Press F5 to build and run.
Make sure your speakers are on. The voice greeting plays when the window opens.

To run from a terminal instead:

bash
dotnet run --project CybersecurityBot.Wpf
Features
Feature	How it works
GUI	Dark-themed WPF window with the ASCII logo as a header, chat bubbles, an input box and a Send button. Press Enter or click Send.
Voice greeting and ASCII logo	welcome.wav plays on launch and the Part 1 logo is shown in a monospaced font.
Typing effect	The bot's replies appear character by character, like the Part 1 console version.
Keyword recognition	11 topics recognised from many keywords each, for example "phish", "2FA", "SIM swap", "VPN", "POPIA".
Random responses	Each topic has several tips. One is picked at random, never the same tip twice in a row, with a randomly chosen intro line.
Conversation flow	Follow-ups such as "another tip", "tell me more", "give me an example" and "I'm confused" continue the current topic without restarting.
Depth ladder	"tell me more" gives an explanation, asking again gives a real-world example, and a third time suggests a new topic.
Memory and recall	Remembers your name and favourite topic, then personalises later tips. Ask "what do you remember" to see it.
Sentiment detection	Detects worried, frustrated, overwhelmed and curious. The bot responds with empathy and gives a tip in the same reply.
Small talk	Greetings, thanks, jokes, "who made you" and more, with several replies each.
Error handling	Empty input shows a warning, unknown input gets a fallback that suggests topics, and a global handler stops unexpected errors from crashing the app.
Topics covered

Passwords, phishing, scams, privacy, malware, safe browsing, social engineering, public Wi-Fi, identity theft, online banking, software updates and backups.

Improvements over Part 1

Part 1 listed some possible improvements. Part 2 delivers these:

Part 1 idea	Part 2 result
Add more topics	4 topics became 11, each with several tips, an explanation and an example
More flexible matching	Many keywords per topic, regex patterns, follow-ups that remember the current topic
Track questions asked	Question count is used in the "what do you remember" reply
Varied conversation	Random tips and intros, multiple replies for small talk, empathetic replies by sentiment
Try these phrases
Type this	What to expect
hello	A greeting using your name
what is phishing	An explanation of phishing
tell me more	A real-world example of the current topic
give me another tip	A different random tip on the same topic
I'm interested in privacy	The bot remembers privacy as your favourite topic
give me a privacy tip	A tip, plus a personal note for your favourite topic
I'm worried about online scams	An empathetic reply and a scam tip in the same message
someone called pretending to be my bank	Recognised as social engineering
what do you remember	Your name, favourite topic and question count
tell me a joke	A random joke
exit	Goodbye message, then the app closes
How the code is organised
CybersecurityBot.sln
├── Cybersecurity_Awareness_Bot_Part_1/   Part 1 console app (unchanged since v1.0.0)
├── CybersecurityBot.Wpf/                 Part 2 WPF app
│   ├── App.xaml / App.xaml.cs            Application entry point and global error handler
│   ├── MainWindow.xaml                   GUI layout: logo, chat area, input row
│   ├── MainWindow.xaml.cs                Chat flow, name capture, typing effect, validation
│   ├── welcome.wav                       Voice greeting
│   ├── Models/
│   │   └── User.cs                       Name, favourite topic, last topic, question count
│   └── Services/
│       ├── AsciiArtService.cs            ASCII logo
│       ├── AudioService.cs               Plays the WAV greeting safely
│       ├── KnowledgeBase.cs              All dictionaries and lists of bot content
│       └── ResponseEngine.cs             Decides how the bot replies
├── docs/                                 Screenshots used in this README
└── .github/workflows/                    GitHub Actions CI
Key techniques
Delegates. ResponseEngine defines delegate string? ResponseHandler(string text) and keeps a List<ResponseHandler> as a pipeline: recall, interest, follow-up, topic or sentiment, then small talk. The first handler that returns a reply wins, so new behaviour is added by writing one method and adding it to the list.
Generic collections. Dictionary<string, string> maps keywords to topics, Dictionary<string, List<string>> holds the random tips, and List<(string Pattern, string[] Replies)> holds small talk.
Separation of concerns. The window only handles display and input, ResponseEngine decides what to say, and KnowledgeBase holds the content. Adding a topic means editing data, not logic.
Regular expressions. Used for small-talk patterns, the "I'm interested in..." memory pattern, and cleaning "my name is..." input.
Async typing effect. async/await with Task.Delay keeps the window responsive while the bot types.
Notes and limitations
Keyword recognition uses simple string matching, so unusual phrasing may reach the fallback reply. Part 3 extends this with broader NLP-style matching.
Memory lasts for the current session only.
The voice greeting is skipped, with a warning in the chat, if welcome.wav is missing or the platform doesn't support playback.
