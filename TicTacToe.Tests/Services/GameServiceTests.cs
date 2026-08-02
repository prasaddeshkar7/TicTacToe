using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;
using TicTacToe.Core.Services;   // <- add the namespace that contains GameService

namespace TicTacToe.Tests.Services
{
    [TestClass]
    public class GameServiceTests
    {
        private Mock<IGameRepository> _gameRepository = null!;
        private Mock<IScoreboardRepository> _scoreboardRepository = null!;
        private Mock<IComputerStrategy> _computerStrategy = null!;

        private GameService _sut = null!;

        [TestInitialize]
        public void Setup()
        {
            _gameRepository = new Mock<IGameRepository>();
            _scoreboardRepository = new Mock<IScoreboardRepository>();
            _computerStrategy = new Mock<IComputerStrategy>();

            _sut = new GameService(_gameRepository.Object, _scoreboardRepository.Object, _computerStrategy.Object);
        }

        [TestMethod]
        [DataRow(GameMode.TwoPlayer)]
        [DataRow(GameMode.Computer)]
        public void CreateGame_ValidMode_CreatesGame(GameMode gameMode)
        {
            //Arrange
            Game game = new Game(gameMode);
            _gameRepository.Setup(g => g.CreateGame(gameMode)).Returns(game);

            //Act
            Game createdGame = _sut.CreateGame(gameMode);

            //Assert
            Assert.AreEqual(createdGame, game);
            Assert.AreEqual(createdGame.Mode, gameMode);
        }

        [TestMethod]
        public void GetGame_GameExists_ReturnsGame()
        {
            // Arrange
            Game game = new Game(GameMode.Computer);
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act
            Game retrievedGame = _sut.GetGame(game.Id);

            // Assert
            Assert.AreEqual(retrievedGame, game);
        }

        [TestMethod]
        public void GetGame_GameDoesNotExist_ThrowsGameNotFoundException()
        {
            // Arrange
            Guid nonExistentGameId = Guid.NewGuid();
            _gameRepository.Setup(g => g.GetGame(nonExistentGameId)).Returns((Game?)null);

            // Act & Assert
            Assert.Throws<GameNotFoundException>(() => _sut.GetGame(nonExistentGameId));
        }

        [TestMethod]
        public void GetScoreboard_ReturnsScoreboard()
        {
            // Arrange
            Scoreboard scoreboard = new Scoreboard();
            _scoreboardRepository.Setup(s => s.GetScoreboard()).Returns(scoreboard);
            // Act
            Scoreboard retrievedScoreboard = _sut.GetScoreboard();
            // Assert
            Assert.AreEqual(retrievedScoreboard, scoreboard);
        }

        [TestMethod]
        public void MakeMove_GameExistsAndValidMove_UpdatesGameAndScoreboard()
        {
            // Arrange
            int expectedMoveCount = 1;
            Game game = new Game(GameMode.TwoPlayer);
            MoveRequest moveRequest = new MoveRequest { Row = 0, Column = 0, Player = Player.X };
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act
            Game updatedGame = _sut.MakeMove(game.Id, moveRequest);

            // Assert
            Assert.AreEqual(updatedGame, game);
            Assert.HasCount(expectedMoveCount, updatedGame.Moves);
            Assert.AreEqual(updatedGame.Moves.Peek().Position.Row, moveRequest.Row);
            Assert.AreEqual(updatedGame.Moves.Peek().Position.Column, moveRequest.Column);
        }

        [TestMethod]
        public void MakeMove_GameDoesNotExist_ThrowsGameNotFoundException()
        {
            // Arrange
            Guid nonExistentGameId = Guid.NewGuid();
            MoveRequest moveRequest = new MoveRequest { Row = 0, Column = 0, Player = Player.X };
            _gameRepository.Setup(g => g.GetGame(nonExistentGameId)).Returns((Game?)null);

            // Act & Assert
            Assert.Throws<GameNotFoundException>(() => _sut.MakeMove(nonExistentGameId, moveRequest));
        }

        [TestMethod]
        public void MakeMove_InvalidMove_ThrowsInvalidMoveException()
        {
            // Arrange
            Game game = new Game(GameMode.TwoPlayer);
            MoveRequest moveRequest = new MoveRequest { Row = 0, Column = 0, Player = Player.X };
            game.MakeMove(moveRequest); // Make the first move
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(game.Id, moveRequest));
        }

        [TestMethod]
        public void ResetGame_GameDoesNotExist_ThrowsGameNotFoundException()
        {
            // Arrange
            Guid nonExistentGameId = Guid.NewGuid();
            _gameRepository.Setup(g => g.GetGame(nonExistentGameId)).Returns((Game?)null);

            // Act & Assert
            Assert.Throws<GameNotFoundException>(() => _sut.ResetGame(nonExistentGameId));
        }

        [TestMethod]
        public void ResetGame_GameExists_ResetsGame()
        {
            // Arrange
            Game game = new Game(GameMode.TwoPlayer);
            game.MakeMove(new MoveRequest { Row = 0, Column = 0, Player = Player.X }); // Make a move to change the state
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);
            _gameRepository.Setup(g => g.ResetGame(game.Id)).Returns(new Game(GameMode.TwoPlayer));

            // Act
            Game resetGame = _sut.ResetGame(game.Id);

            // Assert
            Assert.AreNotEqual(resetGame, game);
            Assert.AreEqual(resetGame.Mode, game.Mode);
            Assert.AreNotEqual(resetGame.Moves.Count, game.Moves.Count);
        }

        [TestMethod]
        public void ResetScoreboard_ResetsScoreboard()
        {
            // Act
            _sut.ResetScoreboard();
            // Assert
            _scoreboardRepository.Verify(s => s.ResetScoreboard(), Times.Once);
        }

        [TestMethod]
        public void Undo_GameDoesNotExist_ThrowsGameNotFoundException()
        {
            // Arrange
            Guid nonExistentGameId = Guid.NewGuid();
            _gameRepository.Setup(g => g.GetGame(nonExistentGameId)).Returns((Game?)null);

            // Act & Assert
            Assert.Throws<GameNotFoundException>(() => _sut.Undo(nonExistentGameId));
        }

        [TestMethod]
        public void Undo_NoMovesToUndo_ThrowsInvalidMoveException()
        {
            // Arrange
            Game game = new Game(GameMode.TwoPlayer);
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.Undo(game.Id));
        }

        [TestMethod]
        public void Undo_GameHasMoves_UndoesLastMove()
        {
            // Arrange
            Game game = new Game(GameMode.TwoPlayer);
            MoveRequest moveRequest = new MoveRequest { Row = 0, Column = 0, Player = Player.X };
            game.MakeMove(moveRequest); // Make a move to have something to undo
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act
            Game updatedGame = _sut.Undo(game.Id);

            // Assert
            Assert.AreEqual(updatedGame, game);
            Assert.HasCount(0, updatedGame.Moves);
        }

        [TestMethod]
        public void Undo_GameHasMovesAndIsWon_ThrowsInvalidMoveException()
        {
            // Arrange
            Game game = new Game(GameMode.TwoPlayer);

            // Simulate a winning condition
            game.MakeMove(new MoveRequest { Row = 0, Column = 0, Player = Player.X });
            game.MakeMove(new MoveRequest { Row = 1, Column = 0, Player = Player.O });
            game.MakeMove(new MoveRequest { Row = 0, Column = 1, Player = Player.X });
            game.MakeMove(new MoveRequest { Row = 1, Column = 1, Player = Player.O });
            game.MakeMove(new MoveRequest { Row = 0, Column = 2, Player = Player.X }); // X wins

            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.Undo(game.Id));
        }

        [TestMethod]
        public void Undo_GameHasMovesAndIsDraw_ThrowsInvalidMoveException()
        {
            // Arrange
            Game game = new Game(GameMode.TwoPlayer);
            // Simulate a draw condition
            game.MakeMove(new MoveRequest { Row = 0, Column = 0, Player = Player.X });
            game.MakeMove(new MoveRequest { Row = 0, Column = 1, Player = Player.O });
            game.MakeMove(new MoveRequest { Row = 0, Column = 2, Player = Player.X });
            game.MakeMove(new MoveRequest { Row = 1, Column = 0, Player = Player.O });
            game.MakeMove(new MoveRequest { Row = 1, Column = 2, Player = Player.X });
            game.MakeMove(new MoveRequest { Row = 1, Column = 1, Player = Player.O });
            game.MakeMove(new MoveRequest { Row = 2, Column = 0, Player = Player.X });
            game.MakeMove(new MoveRequest { Row = 2, Column = 2, Player = Player.O });
            game.MakeMove(new MoveRequest { Row = 2, Column = 1, Player = Player.X }); // Draw

            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act & Assert

            Assert.Throws<InvalidMoveException>(() => _sut.Undo(game.Id));
        }

        [TestMethod]
        public void MakeMove_GameModeComputer_PlayerXMakesMove_ComputerMakesMove()
        {
            // Arrange
            int expectedMoveCount = 2; // One for player and one for computer
            Game game = new Game(GameMode.Computer);
            MoveRequest playerMoveRequest = new MoveRequest { Row = 0, Column = 0, Player = Player.X };
            BoardPosition computerMovePosition = new BoardPosition(1, 1);
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);
            _computerStrategy.Setup(c => c.GetNextPosition(It.IsAny<Player[][]>())).Returns(computerMovePosition);

            // Act
            Game updatedGame = _sut.MakeMove(game.Id, playerMoveRequest);

            // Assert
            Assert.AreEqual(updatedGame, game);
            Assert.HasCount(expectedMoveCount, updatedGame.Moves);
            Assert.AreEqual(updatedGame.Moves.Peek().Position.Row, computerMovePosition.Row);
            Assert.AreEqual(updatedGame.Moves.Peek().Position.Column, computerMovePosition.Column);
        }

        [TestMethod]
        public void MakeMove_GameModeComputer_PlayerOMakesMove_ThrowsInvalidMoveException()
        {
            // Arrange
            Game game = new Game(GameMode.Computer);

            // Player O tries to make a move in a game mode where only Player X is allowed to make the first move
            MoveRequest playerOMoveRequest = new MoveRequest { Row = 0, Column = 0, Player = Player.O };
            _gameRepository.Setup(g => g.GetGame(game.Id)).Returns(game);

            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(game.Id, playerOMoveRequest));
        }

        [TestMethod]
        public void MakeMove_ValidMove_UpdatesRepository()
        {
            // Arrange
            Game game = new(GameMode.TwoPlayer);

            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            MoveRequest request = new()
            {
                Player = Player.X,
                Row = 0,
                Column = 0
            };

            // Act
            _sut.MakeMove(game.Id, request);

            // Assert
            _gameRepository.Verify(
                x => x.UpdateGame(game),
                Times.Once);
        }

        [TestMethod]
        public void MakeMove_TwoPlayerMode_DoesNotInvokeComputerStrategy()
        {
            // Arrange

            Game game = new(GameMode.TwoPlayer);

            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            // Act

            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 0
                });

            // Assert
            _computerStrategy.Verify(
                x => x.GetNextPosition(It.IsAny<Player[][]>()),
                Times.Never);
        }

        [TestMethod]
        public void MakeMove_PlayerWins_UpdatesScoreboard()
        {
            // Arrange
            Game game = new(GameMode.TwoPlayer);
            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            // Act
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 0
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.O,
                    Row = 1,
                    Column = 0
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 1
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.O,
                    Row = 1,
                    Column = 1
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 2
                });

            // Assert
            _scoreboardRepository.Verify(
                x => x.UpdateScoreboard(game),
                Times.Once);
        }

        [TestMethod]
        public void MakeMove_GameDraw_UpdatesScoreboard()
        {
            // Arrange
            Game game = new(GameMode.TwoPlayer);
            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            // Act
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 0
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.O,
                    Row = 0,
                    Column = 1
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 2
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.O,
                    Row = 1,
                    Column = 0
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 1,
                    Column = 2
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.O,
                    Row = 1,
                    Column = 1
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 2,
                    Column = 0
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.O,
                    Row = 2,
                    Column = 2
                });
            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 2,
                    Column = 1
                });

            // Assert
            _scoreboardRepository.Verify(
                x => x.UpdateScoreboard(game),
                Times.Once);
        }

        [TestMethod]
        public void MakeMove_GameStillInProgress_DoesNotUpdateScoreboard()
        {
            // Arrange

            Game game = new(GameMode.TwoPlayer);

            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            // Act

            _sut.MakeMove(game.Id,
                new MoveRequest
                {
                    Player = Player.X,
                    Row = 0,
                    Column = 0
                });

            // Assert

            _scoreboardRepository.Verify(
                x => x.UpdateScoreboard(It.IsAny<Game>()),
                Times.Never);
        }

        [TestMethod]
        public void Undo_ValidMove_UpdatesRepository()
        {
            // Arrange
            Game game = new(GameMode.TwoPlayer);
            game.MakeMove(new MoveRequest { Player = Player.X, Row = 0, Column = 0 });
            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            // Act
            _sut.Undo(game.Id);

            // Assert
            _gameRepository.Verify(
                x => x.UpdateGame(game),
                Times.Once);
        }

        [TestMethod]
        public void Undo_ComputerMode_RemovesTwoMoves()
        {
            // Arrange
            Game game = new(GameMode.Computer);
            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);
            game.MakeMove(new MoveRequest { Player = Player.X, Row = 0, Column = 0 });
            BoardPosition computerMovePosition = new BoardPosition(1, 1);
            game.MakeMove(new MoveRequest { Player = Player.O, Row = computerMovePosition.Row, Column = computerMovePosition.Column });

            // Act
            _sut.Undo(game.Id);

            // Assert
            Assert.IsEmpty(game.Moves);
        }

        [TestMethod]
        public void ResetGame_PreservesScoreboard()
        {
            // Arrange
            Game game = new(GameMode.Computer);
            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);
            game.MakeMove(new MoveRequest { Player = Player.X, Row = 0, Column = 0 });

            // Act
            _sut.ResetGame(game.Id);

            // Assert
            _scoreboardRepository.Verify(
                x => x.UpdateScoreboard(It.IsAny<Game>()),
                Times.Never);
        }


        [TestMethod]
        public void ResetGame_CallsGameRepository()
        {
            // Arrange
            Game game = new(GameMode.Computer);
            _gameRepository
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);
            game.MakeMove(new MoveRequest { Player = Player.X, Row = 0, Column = 0 });

            // Act
            _sut.ResetGame(game.Id);

            // Assert
            _gameRepository.Verify(
                x => x.ResetGame(game.Id),
                Times.Once);
        }

        [TestMethod]
        public void CreateGame_CallsGameRepository()
        {
            //Arrange & Act
            _sut.CreateGame(GameMode.Computer);

            //Assert
            _gameRepository.Verify(
                x => x.CreateGame(GameMode.Computer),
                Times.Once);
        }

        [TestMethod]
        public void GetScoreboard_CallsScoreboardRepository()
        {
            //Arrange and act
            _sut.GetScoreboard();

            // Assert
            _scoreboardRepository.Verify(
                x => x.GetScoreboard(), Times.Once);
        }
    }
}
