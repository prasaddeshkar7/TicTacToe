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
