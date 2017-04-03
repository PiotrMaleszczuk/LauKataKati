using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    [Serializable]
    abstract public class BoardObject
    {
        protected BoardSide boardObjectSide;
        protected BoardObjectType boardObjectType;
        protected bool locked = false;

        public BoardSide getBoardObjectSide()
        {
            return boardObjectSide;
        }

        public BoardObjectType getBoardObjectType()
        {
            return boardObjectType;
        }

        public bool getLocked()
        {
            return locked;
        }

        public void setLocked(bool locked)
        {
            this.locked = locked;
        }
    }
}
