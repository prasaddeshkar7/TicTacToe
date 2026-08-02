# Tic Tac Toe

A full-stack Tic Tac Toe application built using **Angular** and **ASP.NET Core** following **Clean Architecture** principles.

The application allows users to play Tic Tac Toe in either **Two Player** mode or **Computer** mode. The backend acts as the single source of truth and exposes REST APIs that manage the complete game lifecycle, including move validation, win detection, draw detection, undo functionality, move history, and a session-level scoreboard.

The frontend is implemented using Angular standalone components and Angular Material, providing a responsive and user-friendly interface that communicates exclusively with the backend through REST APIs.

## Key Features

* Two Player Mode
* Play Against Computer
* Intelligent computer opponent using a priority-based strategy
* Move history
* Undo support

  * Single move undo in Two Player mode
  * Double move undo (player + computer) in Computer mode
* Win detection
* Draw detection
* Winning cell highlighting
* Session-level scoreboard
* Reset Game
* Reset Scoreboard
* RESTful API
* Global exception handling middleware
* Angular Material user interface
* Unit tests using MSTest and Moq

# Architecture

The solution follows the principles of **Clean Architecture**, separating business logic from infrastructure and presentation concerns. This keeps the core game logic independent, testable, and easy to extend.

```
┌──────────────────────────────────────────────────────────────┐
│                    Angular Frontend                          │
│                                                              │
│  Components  →  Services  →  HttpClient                      │
└───────────────────────────────┬──────────────────────────────┘
                                │ REST API
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                  ASP.NET Core Web API                        │
│                                                              │
│               Controllers (Thin API Layer)                   │
└───────────────────────────────┬──────────────────────────────┘
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                 Application / Service Layer                  │
│                                                              │
│                     GameService                              │
└───────────────────────────────┬──────────────────────────────┘
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                     Domain Layer                             │
│                                                              │
│ Game • Board • Move • Scoreboard • Computer Strategy         │
│ Business Rules                                               │
└───────────────────────────────┬──────────────────────────────┘
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                 Repository Layer                             │
│                                                              │
│ GameRepository • ScoreboardRepository                        │
│ (In-Memory Storage)                                          │
└──────────────────────────────────────────────────────────────┘
```

## Responsibilities

### Angular Frontend

* Displays the game board and scoreboard
* Calls backend REST APIs
* Contains no game rules or validation logic
* Uses Angular Material for the user interface

### Controllers

Controllers are intentionally thin. They only:

* Receive HTTP requests
* Delegate work to `GameService`
* Return appropriate HTTP responses

All business logic resides outside the controllers.

### GameService

`GameService` acts as the application service and orchestrates the application's workflow.

Its responsibilities include:

* Creating new games
* Retrieving game state
* Processing moves
* Triggering computer moves when required
* Updating the scoreboard
* Resetting games
* Undoing moves

### Domain Layer

The domain layer contains the core business rules.

Examples include:

* Move validation
* Turn switching
* Win detection
* Draw detection
* Winning cell calculation
* Board state management

The domain layer has no dependency on ASP.NET Core or Angular.

### Repository Layer

The repositories provide persistence for the application.

For this assessment, an in-memory implementation was chosen as permitted by the specification.

* `GameRepository` stores active games.
* `ScoreboardRepository` stores the session-level scoreboard.

Both repositories are registered as Singleton services so that state is shared across API requests.

# Design Decisions

## Clean Architecture

The application follows Clean Architecture to separate business logic from infrastructure and presentation concerns.

This approach provides the following benefits:

* Business rules remain independent of ASP.NET Core and Angular.
* The core domain can be unit tested without web or persistence dependencies.
* Infrastructure implementations (such as repositories or databases) can be replaced without affecting the domain layer.
* Controllers remain thin and only coordinate HTTP requests and responses.

---

## Repository Pattern

The repository pattern was used to abstract data persistence from the application layer.

Two repositories are provided:

* `GameRepository`
* `ScoreboardRepository`

For this assessment, both repositories use in-memory storage, which satisfies the assessment requirements while keeping the implementation simple.

Future implementations can replace these repositories with a database-backed implementation without changing the application or domain layers.

---

## Dependency Injection

All application services and repositories are registered through ASP.NET Core's built-in dependency injection container.

This provides:

* Loose coupling
* Easier unit testing through mocking
* Clear separation of responsibilities

---

## Singleton Repository Lifetime

The repositories are registered as **Singleton** services.

This decision was made because the assessment requires maintaining a session-level game state and scoreboard while using in-memory storage.

Using Singleton ensures:

* Games persist across HTTP requests
* The scoreboard is shared across all API requests during the application's lifetime

---

## Move History Implementation

Move history is internally stored using a `Stack<Move>`.

A stack is the natural data structure because the primary operation is **Undo**, which follows Last-In-First-Out (LIFO) semantics.

Advantages:

* O(1) insertion (`Push`)
* O(1) undo (`Pop`)
* Simple implementation

Although the UI displays moves chronologically, the underlying storage remains optimized for Undo operations.

---

## Undo Behaviour

The application follows **Option A** from the assessment specification.

* Undo is allowed only while the game is in progress.
* Once a game is won or drawn, Undo is disabled.
* The scoreboard remains final for completed games.

This approach simplifies score consistency and avoids recalculating previously recorded results.

---

## Computer Player Strategy

The computer opponent is implemented using a dedicated strategy component.

The move selection follows the required priority order:

1. Play a winning move if available.
2. Block the opponent's winning move.
3. Take the center cell.
4. Take an available corner.
5. Take the first available empty cell.

Separating this logic into its own strategy class keeps `GameService` focused on orchestration rather than AI decision making.

---

## RESTful API Design

The backend exposes REST APIs for all game operations.

Each endpoint has a single responsibility and uses appropriate HTTP verbs:

* `POST` for operations that create or modify state.
* `GET` for retrieving game or scoreboard state.

The backend acts as the single source of truth, while the Angular application is responsible only for presentation.

---

## Global Exception Handling

A global exception handling middleware was implemented to provide consistent error responses.

Benefits include:

* Centralized exception handling
* Consistent HTTP status codes
* Simplified controller implementation
* Improved maintainability

---

## Unit Testing

The solution includes backend unit tests using MSTest and Moq.

The tests focus on:

* Core game rules
* Application service behaviour
* Computer strategy
* Error scenarios

External dependencies such as repositories and the computer strategy are mocked to ensure fast and isolated unit tests.

# Technologies Used

## Backend

* .NET 10
* ASP.NET Core Web API
* C#
* MSTest
* Moq
* Swagger / OpenAPI

## Frontend

* Angular 22
* TypeScript
* Angular Material
* RxJS
* Signals
* Standalone Components

## Development Tools

* Visual Studio 2026
* Visual Studio Code
* Postman
* Git
* GitHub

---

# Project Structure

```
TicTacToe
│
├── TicTacToe.Api
│   ├── Controllers
│   ├── Middleware
│   ├── Configuration
│   └── Program.cs
│
├── TicTacToe.Core
│   ├── Models
│   ├── Services
│   ├── Interfaces
│   ├── Enums
│   ├── Exceptions
│   └── Repositories
│
├── TicTacToe.Tests
│
└── tic-tac-toe-ui
    ├── Components
    ├── Services
    ├── Models
    ├── Interceptors
    └── Environments
    └── Styles
```

---

# Prerequisites

Before running the application, ensure the following are installed:

* .NET 10 SDK
* Node.js (24.18.1 LTS version)
* Angular CLI (version 22)
* Git

---

# Running the Backend

Clone the backend repo

```bash
git clone https://github.com/prasaddeshkar7/TicTacToe.git
```


Navigate to the backend project.

```bash
cd TicTacToe/TicTacToe.Api
```

Restore packages.

```bash
dotnet restore
```

Run the application.

```bash
dotnet run
```

Swagger will be available at:

```
https://localhost:5225/swagger
```

---

# Running the Frontend

Clone the frontend repo 

```
git clone https://github.com/prasaddeshkar7/TicTacToeUi.git
```

Navigate to the Angular project.

```bash
cd tic-tac-toe-ui
```

Install dependencies.

```bash
npm install
```

Run the application.

```bash
ng serve
```

Open the application in a browser.

```
http://localhost:4200
```

The frontend communicates with the backend REST APIs configured in `environment.ts`.

---

# API Summary

| Method | Endpoint              | Description         |
| ------ | --------------------- | ------------------- |
| POST   | /api/games            | Create a new game   |
| GET    | /api/games/{id}       | Get current game    |
| POST   | /api/games/{id}/moves | Make a move         |
| POST   | /api/games/{id}/undo  | Undo last move      |
| POST   | /api/games/{id}/reset | Reset current game  |
| GET    | /api/scoreboard       | Retrieve scoreboard |
| POST   | /api/scoreboard/reset | Reset scoreboard    |

---

# Running Unit Tests

Run all backend tests using:

Navigate to the backend repo i.e. TicTacToe
Run below command to run all tests in TicTacToe.slnx

```bash
dotnet test
```

The solution includes unit tests covering:

* Game creation
* Valid and invalid moves
* Turn switching
* Win detection
* Draw detection
* Undo behaviour
* Reset functionality
* Scoreboard updates
* Computer move logic
* Error scenarios

---

# AI Usage Summary

AI was used as a development assistant throughout this assessment to improve productivity and accelerate implementation. It was primarily used for discussing design approaches, generating initial code drafts, reviewing implementations, brainstorming test scenarios, and preparing documentation.

The final solution is the result of iterative development and manual review rather than direct acceptance of AI-generated code.

## How AI was used

AI assistance included:

* Discussing the overall solution architecture and implementation strategy.
* Generating initial implementations for repetitive or boilerplate code.
* Explaining framework concepts and best practices for ASP.NET Core and Angular.
* Reviewing implementation ideas and suggesting possible improvements.
* Brainstorming unit test scenarios based on the assessment requirements.
* Assisting in preparing project documentation and the README.

## Human Review and Adaptation

Every AI-generated suggestion was manually reviewed before being incorporated into the project.

Where appropriate, the generated code was modified to better align with my own design decisions, coding style, and understanding of the problem. This included:

* Reviewing all generated code before integration into the solution.
* Refactoring implementations where I considered alternative approaches to be more appropriate.
* Debugging issues manually and identifying root causes rather than relying solely on generated suggestions.
* Verifying functionality through manual testing using Swagger and the Angular frontend.
* Writing and extending unit tests to validate the final implementation.

## Design Decisions Made During Development

Several implementation decisions were intentionally made or refined during development after reviewing AI-generated suggestions. Examples include:

* Implementing the game board using an encapsulated internal representation rather than exposing mutable state directly.
* Choosing a `Stack<Move>` to store move history in order to optimize Undo operations using Last-In-First-Out (LIFO) semantics.
* Using thin API controllers with business logic delegated to dedicated service classes.
* Keeping the project structure intentionally simple while maintaining separation of concerns through dedicated projects for API, domain logic, infrastructure, and tests.
* Introducing global exception handling middleware to centralize error handling.
* Using dependency injection throughout the application to improve modularity and testability.
* Implementing the computer player's move selection as a dedicated strategy component to keep orchestration separate from decision-making logic.

## Testing

AI was used to suggest potential unit test scenarios based on the assessment requirements. Those suggestions were reviewed, refined, and expanded before implementation.

Additional scenarios were added based on my own analysis of the game rules, service orchestration, edge cases, and expected application behaviour to improve overall test coverage.

## Ownership

I remained responsible for:

* Understanding every implemented component.
* Reviewing and adapting generated code.
* Architectural decisions and trade-offs.
* Integration between the backend and frontend.
* Debugging runtime issues.
* Testing and validating the final behaviour.
* Producing the final submission.


---

# Assumptions

The following assumptions were made during implementation:

* A single backend instance manages the in-memory game state.
* Games are maintained only for the lifetime of the running application.
* The scoreboard is session-level and resets when the application restarts.
* The backend is the single source of truth for all game rules and state.
* Undo is disabled once a game has completed (Option A from the assessment).

---

# Known Limitations

* In-memory storage is used instead of persistent storage.
* Multiplayer over a network is not supported.
* Authentication and authorization are not implemented.
* Real-time synchronization between multiple browser sessions is not included.
* The computer opponent follows a deterministic priority-based strategy rather than a Minimax algorithm.
* Unit tests have been implemented for the backend only. Frontend unit tests were not included due to time constraints and are identified as a future improvement.


---

# Future Improvements

Possible enhancements include:

* Replace in-memory repositories with a relational database (SQL Server or SQLite).
* Add user authentication and player profiles.
* Persist game history across application restarts.
* Introduce SignalR for real-time multiplayer gameplay.
* Replace the heuristic computer strategy with a Minimax-based AI.
* Add Docker support.
* Add CI/CD using GitHub Actions.
* Improve accessibility and keyboard navigation.
* Increase backend and frontend test coverage.


