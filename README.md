# Cybersecurity Awareness Bot (Part 3 / POE)

A WPF desktop chatbot that teaches South African users how to stay safe online. Part 3 adds a MySQL-backed task assistant with reminders, a cybersecurity quiz, NLP-style command recognition and an activity log, all inside the Part 2 chatbot.

| | |
|---|---|
| **Module** | Programming 2A (PROG6221) |
| **Student** | Aphiwe Magwaza |
| **Student number** | `ST10499504` |
| **Repository** | https://github.com/aphiwe-debug/Cybersecurity-Awareness-Bot-Part-1 |
| **Part 1 video** | `https://1drv.ms/v/c/6b4b4f53ee999311/IQCwnD-H7RCzTKvrKsIX8U6bAY0j0-r8dKYZ_PqaGligIiE?e=FPktjR` |
| **Part 2 video** | https://1drv.ms/v/c/6b4b4f53ee999311/IQDQ74JwU_7BRpRe4YeI5j9zAVtSl4irff_SGPUOU_KfDg4?e=FDpCRU |
| **Part 3 / POE video** | https://1drv.ms/v/c/6b4b4f53ee999311/IQBqFH47eLIUSpT7ijBDynWvAYYRYWEhXVgtB7CD0q7fXPc?e=1vPbEB |

## Project parts

| Part | What it is | Tag |
|---|---|---|
| Part 1 | Console chatbot with voice greeting, ASCII logo and basic responses | `v1.0.0` |
| Part 2 | WPF GUI, keyword recognition, random tips, memory, sentiment detection | `v2.0.0` |
| **Part 3 / POE** | **Task assistant (MySQL), quiz, NLP simulation, activity log** | **`v3.0.0`** |

##Actions
<img width="796" height="195" alt="image" src="https://github.com/user-attachments/assets/75b89e5a-fbf1-4d2f-bcd8-511d4713e961" />


---

## Requirements

- Windows 10 or 11 (the voice greeting uses `System.Media.SoundPlayer`)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 with the **.NET desktop development** workload
- **MySQL Server 8.x** running locally on port 3306 ([MySQL Installer for Windows](https://dev.mysql.com/downloads/installer/), "Server only" is enough)

## Setup and run

1. Clone the repository:
   ```bash
   git clone https://github.com/aphiwe-debug/Cybersecurity-Awareness-Bot-Part-1.git
   ```
2. Make sure MySQL Server is running and you know the `root` password.
3. In the `CybersecurityBot.Wpf` folder, copy `dbsettings.example.json` to a new file named `dbsettings.json` and put your MySQL password in it:
   ```json
   {
     "Server": "localhost",
     "Port": 3306,
     "User": "root",
     "Password": "YOUR_MYSQL_PASSWORD",
     "Database": "cybersecurity_bot"
   }
   ```
   `dbsettings.json` is excluded from Git so passwords are never committed.
4. Open `CybersecurityBot.sln` in Visual Studio.
5. Right-click **CybersecurityBot.Wpf**, choose **Set as Startup Project**, then press **F5**.

The app creates the `cybersecurity_bot` database and the `tasks` table automatically on first run. A green "Connected to the MySQL database" message appears when it works. If MySQL is unreachable, the app shows a clear message and the chat, quiz and log still work.

## Part 3 features

The window has four tabs: **Chat**, **Tasks**, **Quiz** and **Activity Log**. Everything from Parts 1 and 2 (voice greeting, ASCII logo, keyword recognition, random tips, memory, sentiment detection) still works in the Chat tab.

### 1. Task assistant with reminders (MySQL)

- Add a task with a title, a description and an optional reminder: no reminder, in 1, 3 or 7 days, or a chosen date.
- View all tasks, mark them completed or delete them. Every change is saved to MySQL.
- Reminders pop up in the chat, with a sound, when they become due.
- Tasks can also be created from the chat (see the NLP section).

**Database table**

```sql
CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    description VARCHAR(500) NOT NULL,
    reminder_date DATETIME NULL,
    is_completed TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

All CRUD operations use parameterised queries in `TaskService`.

### 2. Cybersecurity quiz

- 12 questions, a mix of multiple choice and true/false, shown one at a time in random order.
- Instant feedback after each answer with a short explanation.
- A final score with a message, for example "Great job! You're a cybersecurity pro!".

### 3. NLP simulation

The bot recognises requests worded in different ways, using regular expressions and string matching.

| Say something like | What happens |
|---|---|
| `Add task - Review privacy settings` | Task saved, bot asks if you want a reminder |
| `Yes, remind me in 3 days` | Reminder saved on that task |
| `Remind me to update my password tomorrow` | Task and reminder saved in one step |
| `Can you remind me to update my password?` | Task saved, bot asks when |
| `Add a reminder to check my privacy settings next week` | Task and reminder saved |
| `Add a task to enable two-factor authentication` | Task saved with a suitable description |
| `Show my tasks` | Pending tasks listed in the chat |
| `Start quiz` / `quiz me` / `test my knowledge` | Opens and starts the quiz |
| `What have you done for me?` / `Show activity log` | Last 5 actions listed |
| `Show more` | Longer history |

Dates understood: `tomorrow`, `today`, `next week`, `next month`, `in 3 days`, `in two weeks`, `in an hour`, and weekday names such as `on Friday`.

### 4. Activity log

Records tasks added, completed and deleted, reminders set and due, quiz started and completed, and actions the bot took from NLP interpretation, each with a timestamp. The Activity Log tab shows the latest 5 and has a **Show more** button. In chat, "show activity log" shows the last 5 and "show more" shows more.

## Project structure

```
CybersecurityBot.sln
├── Cybersecurity_Awareness_Bot_Part_1/   Part 1 console app (v1.0.0)
├── CybersecurityBot.Wpf/
│   ├── App.xaml / App.xaml.cs            Shared styles, global error handler
│   ├── MainWindow.xaml / .cs             Tabs, chat flow, reminder timer
│   ├── TasksView.xaml / .cs              Task assistant tab
│   ├── QuizView.xaml / .cs               Quiz tab
│   ├── LogView.xaml / .cs                Activity log tab
│   ├── dbsettings.example.json           Template for the MySQL settings
│   ├── Models/                           User, TaskItem, QuizQuestion
│   └── Services/
│       ├── DatabaseSettings.cs           Loads connection settings
│       ├── DatabaseService.cs            Creates the database and table, opens connections
│       ├── TaskService.cs                CRUD for tasks
│       ├── ActivityLog.cs                In-memory log of actions
│       ├── QuizService.cs                Question bank and score feedback
│       ├── AssistantService.cs           NLP simulation for commands
│       ├── ResponseEngine.cs             Part 2 delegate pipeline (keywords, tips, sentiment)
│       ├── KnowledgeBase.cs              Dictionaries of bot content
│       ├── AudioService.cs               Voice greeting
│       └── AsciiArtService.cs            ASCII logo
├── docs/                                 Screenshots
└── .github/workflows/                    GitHub Actions CI
```

### Key techniques

- **Layered design:** views handle display, services hold the logic, and `DatabaseService` is the only class that knows connection details.
- **MySQL with parameterised queries** (`MySql.Data`), so user text can never alter the SQL.
- **Regular expressions** for command recognition, date phrases and title clean-up in `AssistantService`.
- **Delegate pipeline** from Part 2 still decides chat replies, after `AssistantService` has had first chance to match a command.
- **Events and timers:** `ActivityLog` raises a `Changed` event the log tab listens to, and a `DispatcherTimer` checks for due reminders every 20 seconds.
- **Generic collections:** `List<QuizQuestion>`, `Dictionary` keyword maps, `HashSet<int>` of reminders already shown.
- **Graceful errors:** database failures show a message instead of crashing, and a global handler catches anything unexpected.

## Version control and releases

Work was done on feature branches (`part-2`, `part-3`) with `Part 2:` and `Part 3:` commit prefixes, then merged into `master` with merge commits.

| Release | Tag | Contents |
|---|---|---|
| Part 1 | `v1.0.0` | Console chatbot |
| Part 2 | `v2.0.0` | Complete Part 2 |
| Part 3 | `v3.0.0` | Complete POE with integration and reminders |

History by part:

- Part 1: https://github.com/aphiwe-debug/Cybersecurity-Awareness-Bot-Part-1/commits/v1.0.0
- Part 2: https://github.com/aphiwe-debug/Cybersecurity-Awareness-Bot-Part-1/compare/v1.0.0...v2.0.0
- Part 3: https://github.com/aphiwe-debug/Cybersecurity-Awareness-Bot-Part-1/compare/v2.0.0...v3.0.0

### Continuous integration

GitHub Actions restores and builds the solution on `windows-latest` for every push and pull request. The screenshot at the top shows a passing run. The build does not need MySQL.

## Limitations

- MySQL must be installed and running to use the task assistant.
- Command recognition is rule-based (regex and string matching), so very unusual phrasing may fall back to the normal chat reply.
- The activity log and chat memory last for the current session. Tasks are stored permanently in MySQL.
