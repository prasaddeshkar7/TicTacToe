using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Tests.Models
{
    [TestClass]
    public class GameTests
    {
        private Game? _sut;

        void InitializeGame(GameMode gameMode)
        {
            _sut = new Game(gameMode);
        }

        [TestMethod]
        public void Game_Ctor_InitializesProperties()
        {
            // Arrange
            GameMode gameMode = GameMode.Computer;

            // Act
            InitializeGame(gameMode);

            // Assert
            Assert.IsNotNull(_sut);
            Assert.AreEqual(gameMode, _sut.Mode);
            Assert.AreEqual(Player.X, _sut.CurrentPlayer);
            Assert.HasCount(0, _sut.Moves);
            Assert.AreEqual(GameStatus.InProgress, _sut.Status); 
            Assert.IsNotNull(_sut.Board);

            foreach (var row in _sut.Board.GetCurrentBoard())
            {
                Assert.HasCount(3, row);
                foreach (var cell in row)
                {
                    Assert.AreEqual(Player.None, cell);
                }
            }
            Assert.IsNull(_sut.Winner);
            Assert.HasCount(0, _sut.WinningCells);
        }

        [TestMethod]
        public void Game_MakeMove_MakesValidMoveAndUpdatesState()
        {
            // Arrange
            InitializeGame(GameMode.Computer);
            var moveRequest = new MoveRequest() { Player = Player.X, Row = 0, Column = 0 };
            
            // Act
            _sut.MakeMove(moveRequest);
 
            // Assert
            Assert.AreEqual(Player.X, _sut.Board.GetCurrentBoard()[0][0]);
            Assert.HasCount(1, _sut.Moves);
            Assert.AreEqual(GameStatus.InProgress, _sut.Status);
            Assert.IsNull(_sut.Winner);
        }

        [TestMethod]
        public void Game_MakeMove_ThrowsException_WhenGameNotInProgress()
        {
            // Arrange
            InitializeGame(GameMode.Computer);
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 2 }); // X wins
            var moveRequest = new MoveRequest() { Player = Player.O, Row = 2, Column = 2 };
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(moveRequest));
        }

        [TestMethod]
        public void Game_MakeMove_ThrowsException_WhenNotPlayersTurn()
        {
            // Arrange
            InitializeGame(GameMode.Computer);
            var moveRequest = new MoveRequest() { Player = Player.O, Row = 0, Column = 0 }; // O tries to play first
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(moveRequest));
        }

        [TestMethod]
        public void Game_MakeMove_ThrowsException_WhenCellAlreadyOccupied()
        {
            // Arrange
            InitializeGame(GameMode.Computer);
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 0 });
            var moveRequest = new MoveRequest() { Player = Player.O, Row = 0, Column = 0 }; // O tries to play in the same cell
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(moveRequest));
        }

        [TestMethod]
        public void Game_MakeMove_UpdatesWinnerAndStatus_WhenPlayerWins()
        {
            // Arrange
            InitializeGame(GameMode.Computer);
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 1 });
            var winningMoveRequest = new MoveRequest() { Player = Player.X, Row = 0, Column = 2 }; // X wins
            // Act
            _sut.MakeMove(winningMoveRequest);
            // Assert
            Assert.AreEqual(GameStatus.Won, _sut.Status);
            Assert.AreEqual(Player.X, _sut.Winner);
            Assert.HasCount(3, _sut.WinningCells);

            CollectionAssert.AreEquivalent(new[]
            {
                new BoardPosition(0,0),
                new BoardPosition(0,1),
                new BoardPosition(0,2)
            },
            _sut.WinningCells);
        }

        [TestMethod]
        public void Game_MakeMove_UpdatesStatus_WhenGameIsADraw()
        {
            // Arrange
            InitializeGame(GameMode.TwoPlayer);
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 0, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 2 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 1, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 2 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 2, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 2, Column = 0 });
            var drawMoveRequest = new MoveRequest() { Player = Player.X, Row = 2, Column = 2 };
            // Act
            _sut.MakeMove(drawMoveRequest);
            // Assert
            Assert.AreEqual(GameStatus.Draw, _sut.Status);
            Assert.IsNull(_sut.Winner);
            Assert.HasCount(0, _sut.WinningCells);
        }

        [TestMethod]
        public void Game_MakeMove_ValidMove_SwitchesTurn()
        {
            InitializeGame(GameMode.TwoPlayer);

            _sut.MakeMove(new()
            {
                Player = Player.X,
                Row = 0,
                Column = 0
            });

            Assert.AreEqual(Player.O, _sut.CurrentPlayer);
        }

        [TestMethod]
        public void Game_Reset_ResetsGameState()
        {
            // Arrange
            InitializeGame(GameMode.TwoPlayer);
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 0 });
            // Act
            _sut.Reset();
            // Assert
            Assert.AreEqual(GameStatus.InProgress, _sut.Status);
            Assert.IsNull(_sut.Winner);
            Assert.HasCount(0, _sut.Moves);
            Assert.AreEqual(Player.X, _sut.CurrentPlayer);
            foreach (var row in _sut.Board.GetCurrentBoard())
            {
                foreach (var cell in row)
                {
                    Assert.AreEqual(Player.None, cell);
                }
            }

            Assert.IsEmpty(_sut.WinningCells);
        }

        [TestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [DataRow(3, 0)]
        [DataRow(0, 3)]
        public void Game_MakeMove_ThrowsException_WhenInvalidRowOrColumn(int row, int column)
        {
            // Arrange
            InitializeGame(GameMode.TwoPlayer);
            var invalidRowMoveRequest = new MoveRequest() { Player = Player.X, Row = row, Column = column }; // Invalid row or column
            
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(invalidRowMoveRequest));
        }

        [TestMethod]
        public void Game_MakeMove_ThrowsException_WhenPlayerIsNone()
        {
            // Arrange
            InitializeGame(GameMode.TwoPlayer);
            var nonePlayerMoveRequest = new MoveRequest() { Player = Player.None, Row = 0, Column = 0 }; // Player.None
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(nonePlayerMoveRequest));
        }

        [TestMethod]
        public void Game_MakeMove_ThrowsException_WhenBoardIsFull()
        {
            // Arrange
            InitializeGame(GameMode.TwoPlayer);

            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 0, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 0, Column = 2 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 1, Column = 0 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 1, Column = 2 });
            _sut.MakeMove(new MoveRequest() { Player = Player.X, Row = 2, Column = 1 });
            _sut.MakeMove(new MoveRequest() { Player = Player.O, Row = 2, Column = 0 });
            var moveRequestAfterFullBoard = new MoveRequest() { Player = Player.O, Row = 0, Column = 0 }; // Attempt to play after board is full
            
            // Act & Assert
            Assert.Throws<InvalidMoveException>(() => _sut.MakeMove(moveRequestAfterFullBoard));
        }

        [TestMethod]
        public void Game_MakeMove_PlayerWinsByColumn_UpdatesWinner()
        {
            InitializeGame(GameMode.TwoPlayer);

            _sut.MakeMove(new() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new() { Player = Player.O, Row = 0, Column = 1 });

            _sut.MakeMove(new() { Player = Player.X, Row = 1, Column = 0 });
            _sut.MakeMove(new() { Player = Player.O, Row = 1, Column = 1 });

            _sut.MakeMove(new() { Player = Player.X, Row = 2, Column = 0 });

            Assert.AreEqual(GameStatus.Won, _sut.Status);
            Assert.AreEqual(Player.X, _sut.Winner);
        }

        [TestMethod]
        public void Game_MakeMove_PlayerWinsByDiagonal_UpdatesWinner()
        {
            InitializeGame(GameMode.TwoPlayer);
            _sut.MakeMove(new() { Player = Player.X, Row = 0, Column = 0 });
            _sut.MakeMove(new() { Player = Player.O, Row = 0, Column = 1 });

            _sut.MakeMove(new() { Player = Player.X, Row = 1, Column = 1 });
            _sut.MakeMove(new() { Player = Player.O, Row = 1, Column = 0 });

            _sut.MakeMove(new() { Player = Player.X, Row = 2, Column = 2 });

            Assert.AreEqual(GameStatus.Won, _sut.Status);
            Assert.AreEqual(Player.X, _sut.Winner);
        }

        [TestMethod]
        public void Game_MakeMove_InvalidMove_DoesNotSwitchTurn()
        {
            InitializeGame(GameMode.TwoPlayer);

            _sut.MakeMove(new()
            {
                Player = Player.X,
                Row = 0,
                Column = 0
            });

            Assert.Throws<InvalidMoveException>(() =>
                _sut.MakeMove(new()
                {
                    Player = Player.O,
                    Row = 0,
                    Column = 0
                }));

            Assert.AreEqual(Player.O,
                _sut.CurrentPlayer);
        }

        [TestMethod]
        public void Game_MakeMove_UpdatesMovesStack()
        {
            InitializeGame(GameMode.TwoPlayer);
            MoveRequest moveRequest1 = new() { Player = Player.X, Row = 0, Column = 0 };
            _sut.MakeMove(moveRequest1);
            MoveRequest moveRequest2 = new() { Player = Player.O, Row = 0, Column = 1 };
            _sut.MakeMove(moveRequest2);

            Assert.HasCount(2, _sut.Moves);
            Assert.AreEqual(Player.O, _sut.Moves.Peek().Player);
            Assert.AreEqual(new BoardPosition(0, 1), _sut.Moves.Peek().Position);

            var lastMove = _sut.Moves.Pop();
            Assert.AreEqual(Player.O, lastMove.Player);
            Assert.AreEqual(0, lastMove.Position.Row);
            Assert.AreEqual(1, lastMove.Position.Column);

            lastMove = _sut.Moves.Pop();
            Assert.AreEqual(Player.X, lastMove.Player);
            Assert.AreEqual(0, lastMove.Position.Row);
            Assert.AreEqual(0, lastMove.Position.Column);
        }
    }
}
