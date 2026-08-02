using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Core.Models
{
    // Represents a position on the Tic Tac Toe board with row and column indices.
    public record struct BoardPosition(int Row, int Column);
}
