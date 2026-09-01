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
