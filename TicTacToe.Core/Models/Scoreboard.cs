using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Core.Models
{
    // <summary>
    // Represents the scoreboard for a Tic Tac Toe game.
    // </summary>
    public class Scoreboard
    {
        public int XWins { get; private set; }

        public int OWins { get; private set; }

        public int Draws { get; private set; }

        public void RegisterXWin() => XWins++;

        public void RegisterOWin() => OWins++;

        public void RegisterDraw() => Draws++;

        public void Reset()
        {
            XWins = 0;
            OWins = 0;
            Draws = 0;
        }

        public void UpdateScoreboard(Scoreboard scoreboard)
        {
            XWins = scoreboard.XWins;
            OWins = scoreboard.OWins;
            Draws = scoreboard.Draws;
        }
    }
}
