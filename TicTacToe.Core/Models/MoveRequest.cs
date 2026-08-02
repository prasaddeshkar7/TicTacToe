using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;

namespace TicTacToe.Core.Models
{
    public class MoveRequest
    {
        public Player Player { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
