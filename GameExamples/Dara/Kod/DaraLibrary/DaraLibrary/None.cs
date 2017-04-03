using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    [Serializable]
    public class None : BoardObject
    {
        public None()
        {
            this.boardObjectSide = BoardSide.NONE;
            this.boardObjectType = BoardObjectType.NONE;
        }
    }
}
