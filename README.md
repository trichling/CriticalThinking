# Critical Thinking Training Game

A web-based game to help people train their critical thinking skills by identifying logical fallacies in text passages.

## Project Structure

- `src/CriticalThinking.Frontend/` - Vue3 SPA frontend
- `src/CriticalThinking.Backend/` - .NET WebAPI backend
- `src/CriticalThinking.Database/` - Database scripts and configuration
- `src/CriticalThinking.Tests/` - Unit tests
- `docs/` - Documentation

## Game Features

- 3 difficulty levels (Easy: 3 fallacies, Medium: 6 fallacies, Hard: 9 fallacies)
- 24 logical fallacies categorized by difficulty
- Time-based scoring system
- Detailed feedback on correct, wrong, and missed fallacies

## Technology Stack

- Frontend: Vue3 SPA
- Backend: .NET WebAPI
- Database: PostgreSQL
- Orchestration: .NET Aspire

## Getting Started

1. Prerequisites: .NET 8 SDK, Node.js, PostgreSQL
2. Run with .NET Aspire: `dotnet run --project src/CriticalThinking.AppHost`