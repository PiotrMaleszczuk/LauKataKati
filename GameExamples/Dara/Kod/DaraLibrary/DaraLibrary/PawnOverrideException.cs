using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    public class PawnOverrideException : Exception
    {
        public PawnOverrideException()
        { }

        public PawnOverrideException(string message)
            : base(message)
        { }
    }
}
