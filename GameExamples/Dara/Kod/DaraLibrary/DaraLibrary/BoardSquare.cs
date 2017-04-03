using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    [Serializable]
    public class BoardSquare
    {
        private int rowCoord;
        private int columnCoord;
        private BoardObject currentObject;

        public BoardSquare(int rowCoord, int columnCoord)
        {
            this.rowCoord = rowCoord;
            this.columnCoord = columnCoord;
            this.currentObject = new None();
        }

        public int getRowCoord()
        {
            return rowCoord;
        }

        public int getColumnCoord()
        {
            return columnCoord;
        }

        public BoardObject getCurrentObject()
        {
            return currentObject;
        }

        public BoardObject takeCurrentObject()
        {
            BoardObject co = currentObject;
            currentObject = new None();
            return co;
        }

        public int setCurrentObject(BoardObject currentObject)
        {
            

            try
            {
                if (this.currentObject is None)
                {
                    this.currentObject = currentObject;
                    return 1;
                }
                else
                {
                    return 2;
                   // throw new PawnOverrideException("Existing object override requested... row: " + this.getRowCoord() + " column: " + this.getColumnCoord());
                    
                }
            }
            catch (PawnOverrideException ex)
            {
               
               // System.Console.WriteLine(ex.Message);
                
            }
            return 0;
        }

        
    }
}
