using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Interfaces
{
    public interface IScoreboardRepository
    {
        Scoreboard GetScoreboard();
        void UpdateScoreboard(Scoreboard scoreboard);
        void UpdateScoreboard(Game game);

        void ResetScoreboard();
    }
}
