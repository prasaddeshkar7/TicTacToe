using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;

namespace TicTacToe.Core.Models
{
    /// <summary>
    /// Represents the state of a Tic Tac Toe game, including the game board, current player, game mode, status, winner, winning cells, and moves made.
    /// </summary>
    public class Game
    {
        public Game(GameMode mode)
        {
            Mode = mode;
        }

        public Guid Id { get; init; } = Guid.NewGuid();

        public Board Board { get; } = new Board();

        public Player CurrentPlayer { get; private set; } = Player.X;

        public GameMode Mode { get; init; }

        public GameStatus Status { get; private set; } = GameStatus.InProgress;

        public Player? Winner { get; private set; }

        public List<BoardPosition> WinningCells { get; private set; } = [];

        public Stack<Move> Moves { get; } = new();

        /// <summary>
        /// Attempts to make a move in the game based on the provided MoveRequest. 
        /// Validates the game status and current player before making the move.
        /// If the move is valid, it updates the game board and records the move.
        /// </summary>
        /// <param name="moveRequest"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MakeMove(MoveRequest moveRequest)
        {
            if (Status != GameStatus.InProgress)
            {
                throw new InvalidMoveException("Cannot make a move when the game is not in progress.");
            }
            if (moveRequest.Player != CurrentPlayer || moveRequest.Player == Player.None)
            {
                throw new InvalidMoveException($"It's not {moveRequest.Player}'s turn.");
            }
            Board.MakeMove(moveRequest);
            Moves.Push(new Move(Moves.Count + 1, moveRequest.Player, new BoardPosition(moveRequest.Row, moveRequest.Column)));
            Player winner  = Board.CheckWinner();

            if(winner != Player.None)
            {
                MarkWinner(winner, Moves.Where(move => move.Player == winner).Select(move => move.Position).ToList());
            }
            else if (Board.IsFull())
            {
                MarkDraw();
            }
            else
            {
               SwitchTurn();
            }
        }

        public void SwitchTurn()
        {
            CurrentPlayer = CurrentPlayer == Player.X
                ? Player.O
                : Player.X;
        }

        public void MarkWinner(Player winner, List<BoardPosition> cells)
        {
            Winner = winner;
            Status = GameStatus.Won;

            WinningCells = cells;
        }

        public void MarkDraw()
        {
            Status = GameStatus.Draw;
        }

        public void Reset()
        {
            Board.Reset();

            Moves.Clear();

            WinningCells = [];

            Winner = null;

            Status = GameStatus.InProgress;

            CurrentPlayer = Player.X;
        }

        public void UndoMove(Move lastButOneMove)
        {
            Board.UndoMove(lastButOneMove);
        }
    }
}
