using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;

namespace TicTacToe.Core.Models
{
    public class Board
    {
        private readonly Player[][] _cells = [new Player[3],
         new Player[3],
         new Player[3]];

        public Player[][] Cells => _cells;

        public Player[][] GetCurrentBoard() 
        { 
            return Cells; 
        }

        public void Reset()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Cells[row][ col] = Player.None;
                }
            }
        }

        public bool IsFull()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (Cells[row][ col] == Player.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValidCell(int row, int col)
        {
            return 0 <= row && row < Cells.Length
                && 0 <= col && col < Cells[row].Length
                && Cells[row][col] == Player.None;
        }
        private bool IsCellEmpty(int row, int col)
        {
            return Cells[row][col] == Player.None;
        }

        public void MakeMove(MoveRequest moveRequest)
        {
            if (IsValidCell(moveRequest.Row, moveRequest.Column) 
                && IsCellEmpty(moveRequest.Row, moveRequest.Column))
            {
                Cells[moveRequest.Row][ moveRequest.Column] = moveRequest.Player;
           
            }
            else
            {
                throw new InvalidMoveException("Cell is already occupied.");
            }
        }

        public Player CheckWinner()
        {
            // Check rows and columns
            for (int i = 0; i < 3; i++)
            {
                if (Cells[i][0] != Player.None && Cells[i][0] == Cells[i][1] && Cells[i][1] == Cells[i][2])
                {
                    return Cells[i][0];
                }
                if (Cells[0][i] != Player.None && Cells[0][i] == Cells[1][i] && Cells[1][i] == Cells[2][i])
                {
                    return Cells[0][i];
                }
            }

            // Check diagonals
            if (Cells[0][0] != Player.None && Cells[0][0] == Cells[1][1] && Cells[1][1] == Cells[2][2])
            {
                return Cells[0][0];
            }
            if (Cells[0][2] != Player.None && Cells[0][2] == Cells[1][1] && Cells[1][1] == Cells[2][0])
            {
                return Cells[0][2];
            }

            return Player.None;
        }

        public void UndoMove(Move moveToBeUndone)
        {
            Cells[moveToBeUndone.Position.Row][moveToBeUndone.Position.Column] = Player.None;
        }
    }
}
