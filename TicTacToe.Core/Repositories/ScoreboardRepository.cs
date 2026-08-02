using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Repositories
{
    public sealed class ScoreboardRepository : IScoreboardRepository
    {
        private readonly Scoreboard _scoreboard = new();

        public Scoreboard GetScoreboard()
        {
            return _scoreboard;
        }

        public void ResetScoreboard()
        {
            _scoreboard.Reset();
        }

        public void UpdateScoreboard(Scoreboard scoreboard)
        {
            _scoreboard.UpdateScoreboard(scoreboard);
        }

        public void UpdateScoreboard(Game game)
        {
            if (game.Status == GameStatus.InProgress)
                return;

            switch (game.Status)
            {
                case GameStatus.Won when game.Winner == Player.X:
                    _scoreboard.RegisterXWin();
                    break;

                case GameStatus.Won when game.Winner == Player.O:
                    _scoreboard.RegisterOWin();
                    break;

                case GameStatus.Draw:
                    _scoreboard.RegisterDraw();
                    break;
            }
        }
    }
}
