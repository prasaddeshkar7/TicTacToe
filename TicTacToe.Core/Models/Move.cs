using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;

namespace TicTacToe.Core.Models
{
    // <summary>
    // Represents a move made by a player in the game.
    // </summary>
    public class Move
    {
        public Player Player { get; init; }

        public Move(int moveNumber, Player player, BoardPosition position)
        {
            MoveNumber = moveNumber;
            Player = player;
            Position = position;
        }

        public int MoveNumber { get; init; }


        public BoardPosition Position { get; init; }

        public DateTime PlayedAt { get; init; } = DateTime.UtcNow;
    }
}
