# Cybersecurity Awareness Bot

A C# console chatbot that educates users on basic cybersecurity practices — passwords, phishing, safe browsing, and malware — through a simple, styled command-line conversation.

## Features

- **ASCII art banner** — displays a logo when the app starts
- **Voice greeting** — plays a `.wav` welcome sound after the intro text (Windows only, via `System.Media.SoundPlayer`)
- **Styled console UI** — colour-coded bot/user messages, dividers, headers, and a typewriter text effect
- **Personalized conversation** — asks for the user's name and uses it throughout the session
- **Keyword-based Q&A engine** — recognizes topics like passwords, phishing, safe browsing, and malware, and responds with relevant tips
- **Input validation** — handles empty/whitespace input and unrecognized questions gracefully
- **Exit commands** — type `exit`, `quit`, or `bye` at any time to end the session

## Project Structure

Each class lives in its own file (not all crammed into `Program.cs`):

| File | Responsibility |
|---|---|
| `Program.cs` | Entry point — wires everything together and runs the main conversation loop |
| `User.cs` | Stores the user's name and a running count of questions asked |
| `ConsoleUI.cs` | Static helper for colours, dividers, headers, typing effect, and prompts |
| `AsciiArtService.cs` | Displays the ASCII logo on startup |
| `AudioService.cs` | Plays the welcome `.wav` file, with graceful fallback if missing/unsupported |
| `ResponseService.cs` | Core chatbot logic — validates input and maps keywords to responses |

## Requirements

- **.NET SDK** (6.0 or later recommended)
- **Windows OS** for the voice greeting feature (`SoundPlayer` is Windows-only; the app still runs on other platforms, just without audio)
- A `welcome.wav` audio file placed in the application's base directory (next to the executable)

## Setup & Running

1. Clone or download the project files.
2. Make sure **all six `.cs` files** (`Program.cs`, `User.cs`, `ConsoleUI.cs`, `AsciiArtService.cs`, `AudioService.cs`, `ResponseService.cs`) are in the **same project folder** as your `.csproj` file, and that each shows up in Solution Explorer with Build Action set to **Compile**. If a file isn't part of the project, you'll get `CS0103`/`CS0246` "does not exist in the current context" errors.
3. Place a `welcome.wav` file in the project's output/base directory (same folder as the compiled `.exe`, or the working directory when run via `dotnet run`).
4. Build and run:
   ```bash
   dotnet build
   dotnet run
   ```
5. Follow the prompts: enter your name, then ask about cybersecurity topics.

## Usage

Once running, you can ask things like:

- `What's a good password?`
- `How do I spot a phishing email?`
- `Is this link safe to click?`
- `What is malware?`
- `What can I ask you?`

Type `exit`, `quit`, or `bye` to end the conversation.

## Continuous Integration

<!-- Paste your GitHub Actions successful (green check) run screenshot here before submitting -->

## Notes

- The chatbot matches responses using simple keyword detection (`Contains`), not natural language processing — phrasing matters.
- If a message doesn't match any known topic, the bot asks the user to rephrase.
- Audio playback failures (missing file, unsupported platform) are caught and logged as warnings without crashing the app.
- You may see a `CA1416` build warning noting `SoundPlayer` is Windows-only — this is expected since the app targets Windows and can be safely ignored.

## Possible Improvements

- Add more topics (e.g. two-factor authentication, social engineering, ransomware)
- Track and summarize `QuestionsAsked` at session end
- Replace keyword matching with a more flexible NLP/intent-matching approach
- Cross-platform audio support
