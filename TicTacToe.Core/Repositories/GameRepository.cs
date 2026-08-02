using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Repositories
{
    public sealed class GameRepository : IGameRepository
    {
        private readonly ConcurrentDictionary<Guid, Game> _games = new();

        public Game CreateGame(GameMode mode)
        {
            var game = new Game(mode);

            if (!_games.TryAdd(game.Id, game))
            {
                // Return the existing game if it already exists
                throw new Exception($"Game of mode '{game.Mode}' could not be created.");
            }

            return game;
        }

        public Game? GetGame(Guid id)
        {
            _games.TryGetValue(id, out var game);
            return game;
        }

        public void UpdateGame(Game game)
        {
            _games[game.Id] = game;
        }

        public void DeleteGame(Guid id)
        {
            _games.TryRemove(id, out _);
        }

        public Game ResetGame(Guid gameId)
        {
            if (!_games.TryGetValue(gameId, out var game))
            {
                throw new GameNotFoundException(gameId);
            }

            game.Reset();

            _games[gameId] = game;

            return game;
        }
    }
}