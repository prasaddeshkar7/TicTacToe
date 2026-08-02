using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException() : base()
        {
        
        }

        public GameNotFoundException(string message) : base(message)
        {
            
        }

        public GameNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            
        }

        public GameNotFoundException(Guid gameId) : base($"Game not found for ID: {gameId}")
        {
            
        }
    }
}
