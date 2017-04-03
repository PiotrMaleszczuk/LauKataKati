using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    [Serializable]
    public class Move
    {
        private Board board;
        private BoardSquare from;
        private BoardSquare to;
        private bool valid;
        private double rating;

        public Move(Board board, BoardSquare from, BoardSquare to, double rating = 0)
        {
            this.board = board;
            this.from = from;
            this.to = to;
            this.valid = validityCheck(from, to);
            this.rating = rating;
        }

        public Move(Board board, BoardSide side)
        {
            this.board = board;

            if (side == BoardSide.MAX)
            {
                rating = UtilityRating.MIN_RATING;
            }

            if (side == BoardSide.MIN)
            {
                rating = UtilityRating.MAX_RATING;
            }
        }

        public Board getBoard()
        {
            return board;
        }

        public bool isValid()
        {
            return valid;
        }

        public double getRating()
        {
            return rating;
        }

        public void setRating(double rating)
        {
            this.rating = rating;
        }

        private static bool validityCheck(BoardSquare from, BoardSquare to) 
        {
            if (to.getCurrentObject() is None) 
            {
                if (isNeighbour(from, to) && isOnBoard(from, to) && !isLocked(from))
                {
                    return true;
                }
            }   
        
            return false;
        }

        private static bool isNeighbour(BoardSquare from, BoardSquare to)
        {
            if (((from.getRowCoord() + 1 == to.getRowCoord() && from.getColumnCoord() == to.getColumnCoord()) ||
                 (from.getRowCoord() - 1 == to.getRowCoord() && from.getColumnCoord() == to.getColumnCoord())) ^
                ((from.getColumnCoord() + 1 == to.getColumnCoord() && from.getRowCoord() == to.getRowCoord()) ||
                 (from.getColumnCoord() - 1 == to.getColumnCoord() && from.getRowCoord() == to.getRowCoord())))
            {
                return true;
            }

            return false;
        }

        private static bool isOnBoard(BoardSquare from, BoardSquare to)
        {
            if ((from.getRowCoord() >= 0 && from.getRowCoord() < Board.BOARD_ROW_COUNT) &&
                (from.getColumnCoord() >= 0 && from.getColumnCoord() < Board.BOARD_COLUMN_COUNT) &&
                (to.getRowCoord() >= 0 && to.getRowCoord() < Board.BOARD_ROW_COUNT) &&
                (to.getColumnCoord() >= 0 && to.getColumnCoord() < Board.BOARD_COLUMN_COUNT))
            {
                return true;
            }

            return false;
        }

        private static bool isLocked(BoardSquare from)
        {
            return from.getCurrentObject().getLocked();
        }

        /*
        private static bool isFieldFree(BoardSquare to)
        {
            if (to.getCurrentObject() is None)
            {
                return true;
            }

            return false;
        }
        */


        public int executeMove()
        {
            if (valid)
            {
                BoardObject co = from.takeCurrentObject();
                to.setCurrentObject(co);
                int reutrnedfours = board.findFours();
                if (reutrnedfours == 1)
                {
                    from.setCurrentObject(co);
                    to.takeCurrentObject();
                    return 1;
                }
                //board.toggleTurnOwner();
                board.setLastMove(this);
                board.isThrees(to);
                
                return 0;
                
            }
            else
            {
                //throw new InvalidMoveExecutionException("Invalid move execution requested :(.");
            }
            return -1;
        }
    }
}
