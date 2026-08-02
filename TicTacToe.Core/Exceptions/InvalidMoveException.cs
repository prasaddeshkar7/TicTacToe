using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Core.Exceptions
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() : base()
        {
        
        }

        public InvalidMoveException(string message) : base(message)
        {
            
        }

        public InvalidMoveException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
