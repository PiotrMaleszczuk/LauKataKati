using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    public class InvalidMoveExecutionException : Exception
    {
        public InvalidMoveExecutionException()
        { }

        public InvalidMoveExecutionException(string message)
            : base(message)
        { }
    }
}
