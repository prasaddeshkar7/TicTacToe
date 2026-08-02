using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Interfaces
{
    // Returns a named tuple with Row and Column so existing usage (computerMove.Row) continues to work.
    public interface IComputerStrategy
    {
        BoardPosition GetNextPosition(Player[][] players);
    }
}
