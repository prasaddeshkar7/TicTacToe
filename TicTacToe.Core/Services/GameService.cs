using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IScoreboardRepository _scoreboardRepository;
        private readonly IComputerStrategy _computerStrategy;

        public GameService(
            IGameRepository gameRepository,
            IScoreboardRepository scoreboardRepository, 
            IComputerStrategy computerStrategy)
        {
            ArgumentNullException.ThrowIfNull(gameRepository, nameof(gameRepository));
            ArgumentNullException.ThrowIfNull(scoreboardRepository, nameof(scoreboardRepository));
            ArgumentNullException.ThrowIfNull(computerStrategy, nameof(computerStrategy));  

            _gameRepository = gameRepository;
            _scoreboardRepository = scoreboardRepository;
            _computerStrategy = computerStrategy;
        }

        public Game CreateGame(GameMode mode)
        {
            return _gameRepository.CreateGame(mode);
        }

        public Game GetGame(Guid gameId)
        {
            Game? game = _gameRepository.GetGame(gameId);
            if (game == null)
            {
                throw new GameNotFoundException(gameId);
            }
            return game;
        }

        public Scoreboard GetScoreboard()
        {
            return _scoreboardRepository.GetScoreboard();
        }

        public Game MakeMove(Guid gameId, MoveRequest request)
        {
            Game? game = _gameRepository.GetGame(gameId);

            if (game == null) 
            {
                throw new GameNotFoundException(gameId);
            }

            game.MakeMove(request);

            if (game.Status == GameStatus.InProgress &&
                game.Mode == GameMode.Computer &&
                game.CurrentPlayer == Player.O)
            {
                BoardPosition computerMove = _computerStrategy.GetNextPosition(game.Board.GetCurrentBoard());

                game.MakeMove(new MoveRequest { Column = computerMove.Column, Row = computerMove.Row, Player = Player.O });
            }

            UpdateScoreboardIfNeeded(game);

            _gameRepository.UpdateGame(game);

            return game;
        }

        private void UpdateScoreboardIfNeeded(Game game)
        {
            if (game.Status == GameStatus.Won || game.Status == GameStatus.Draw)
            {
                _scoreboardRepository.UpdateScoreboard(game);
            }
        }

        public Game ResetGame(Guid gameId)
        {
            Game? game = _gameRepository.GetGame(gameId);

            if (game == null)
            {
                throw new GameNotFoundException(gameId);
            }

            return _gameRepository.ResetGame(gameId);
        }

        public void ResetScoreboard()
        {
            _scoreboardRepository.ResetScoreboard();
        }

        public Game Undo(Guid gameId)
        {
            Game? game = _gameRepository.GetGame(gameId);
            if (game == null)
            {
                throw new GameNotFoundException(gameId);
            }

            if (game.Moves.Count == 0)
            {
                throw new InvalidMoveException("No moves to undo.");
            }

            if (game.Status == GameStatus.Draw || game.Status == GameStatus.Won)
            {
               throw new InvalidMoveException("Cannot undo a move after the game has ended.");
            }

            // Remove the last move

            if (game.Mode == GameMode.TwoPlayer) {
                Move lastMove = game.Moves.Pop();
                game.UndoMove(lastMove);
                game.SwitchTurn();
            }
            else
            {
                Move lastMove = game.Moves.Pop();
                game.UndoMove(lastMove);
                Move lastButOneMove = game.Moves.Pop();
                game.UndoMove(lastButOneMove);
            }

            _gameRepository.UpdateGame(game);
            return game;
        }
    }
}
