using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Interfaces
{
    public interface IGameService
    {
        Game CreateGame(GameMode mode);

        Game GetGame(Guid gameId);

        Game MakeMove(Guid gameId, MoveRequest request);

        Game Undo(Guid gameId);

        Game ResetGame(Guid gameId);

        Scoreboard GetScoreboard();

        void ResetScoreboard();
    }
}
