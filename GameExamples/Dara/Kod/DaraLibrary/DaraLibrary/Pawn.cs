using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    [Serializable]
    public class Pawn : BoardObject
    {
        public Pawn(BoardSide boardObjectSide)
        {
            this.boardObjectSide = boardObjectSide;
            this.boardObjectType = BoardObjectType.PAWN;
        }
    }
}
