using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Interfaces
{
    public interface IGameRepository
    {
        Game CreateGame(GameMode mode);

        Game? GetGame(Guid id);

        void UpdateGame(Game game);

        void DeleteGame(Guid id);
        Game ResetGame(Guid gameId);
    }
}
