# 🪄 spell — Command-Line Assistant

**spell** — a lightweight, modular CLI utility for performing tasks in natural language.

It supports reminders, timers, unit conversion, and note-taking.

Under the hood — a hybrid NLP system combining Keyword Matching, TF-IDF, and Cosine Similarity, working entirely offline without any internet connection.

---

## Quick Start

### Build and Run

```bash
dotnet build
dotnet run spell cast "remind me to call mom tomorrow"
```

---

## ⚙️ Commands

All commands start with the word **spell**.

Then comes the subcommand **cast**, followed by a natural language description of the action.

```bash
spell cast "set a timer for 10 minutes"
spell cast "convert 10 kilometers to miles"
spell cast "take a note: project deadline next week"
```

---

## 🧩 Architecture

```
+------------------------+
|        CLI Core        |
+------------------------+
          |
          v
+-------------------------------+
|  NLP Pipeline                 |
|  - KeywordIntentClassifier    |
|  - SemanticIntentClassifier   |
|  - HybridClassifier           |
|  - RecognizersEntityExtractor |
+-------------------------------+
          |
          v
+------------------------+
|     Modules Layer      |
|  - ReminderModule      |
|  - TimerModule         |
|  - NotesModule         |
|  - ConverterModule     |
+------------------------+
```

---

## 🧠 Core Components

- **HybridClassifier** — combines keyword and semantic analysis for more accurate intent detection.
- **NlpPipeline** — processes input commands and routes results to the appropriate module.
- **Modules** — perform specific actions (reminders, timers, notes, conversions).

---

## 🗂️ Project Structure

```
spell/
│
├── Core/
|   ├── examples.json
│   ├── NlpPipeline.cs
│   ├── HybridClassifier.cs
│   ├── KeywordIntentClassifier.cs
│   ├── SemanticIntentClassifier.cs
│   └── RecognizersEntityExtractor.cs
│
├── Modules/
│   ├── ReminderModule.cs
│   ├── TimerModule.cs
│   ├── NotesModule.cs
│   └── ConverterModule.cs
│
└── Program.cs
```
