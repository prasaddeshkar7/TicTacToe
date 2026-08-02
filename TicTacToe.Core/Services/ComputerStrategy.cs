using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Services
{
    public class ComputerStrategy : IComputerStrategy
    {
        public BoardPosition GetNextPosition(Player[][] board)
        {
            // 1. Win if possible
            BoardPosition? nextPosition = FindWinningMove(board, Player.O);
            if (nextPosition != null)
                return (BoardPosition)nextPosition;

            // 2. Block opponent
            nextPosition = FindWinningMove(board, Player.X);
            if (nextPosition != null)
                return (BoardPosition)nextPosition;

            // 3. Take center
            if (board[1][1] == Player.None)
                return new BoardPosition(1, 1);

            // 4. Take any corner
            int[][] corners =
            [[0,0],
            [0,2],
            [2,0],
            [2,2]];

            foreach (var corner in corners)
            {
                if (board[corner[0]][corner[1]] == Player.None)
                    return new BoardPosition(corner[0], corner[1]);
            }

            // 5. Take first available cell
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row][col] == Player.None)
                        return new BoardPosition(row, col);
                }
            }

            throw new InvalidMoveException("Board is full.");
        }

        private BoardPosition? FindWinningMove(Player[][] board, Player player)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row][col] != Player.None)
                        continue;

                    board[row][col] = player;

                    if (HasWon(board, player))
                    {
                        board[row][col] = Player.None;
                        return new BoardPosition(row, col);
                    }

                    board[row][col] = Player.None;
                }
            }

            return null;
        }

        private static bool HasWon(Player[][] board, Player player)
        {
            // Rows
            for (int row = 0; row < 3; row++)
            {
                if (board[row][0] == player &&
                    board[row][1] == player &&
                    board[row][2] == player)
                    return true;
            }

            // Columns
            for (int col = 0; col < 3; col++)
            {
                if (board[0][col] == player &&
                    board[1][col] == player &&
                    board[2][col] == player)
                    return true;
            }

            // Main diagonal
            if (board[0][0] == player &&
                board[1][1] == player &&
                board[2][2] == player)
                return true;

            // Anti diagonal
            if (board[0][2] == player &&
                board[1][1] == player &&
                board[2][0] == player)
                return true;

            return false;
        }
    }
}
