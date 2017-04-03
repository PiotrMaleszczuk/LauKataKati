using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    public class NoneTurnException : Exception
    {
        public NoneTurnException()
        { }

        public NoneTurnException(string message)
            : base(message)
        { }
    }
}
