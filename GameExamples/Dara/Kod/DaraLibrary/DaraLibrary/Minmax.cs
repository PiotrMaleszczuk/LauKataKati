using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    public class Minmax
    {
        private static int MAX_DEPTH = 1;

        public static Move getNextMove(Board board)
        {
            Move nextMove;

            if (board.getTurnOwner() == BoardSide.MAX)
            {
                nextMove = maxValue(board, 0);
            }
            else if (board.getTurnOwner() == BoardSide.MIN)
            {
                nextMove = minValue(board, 0);
            }
            else
            {
                throw new NoneTurnException("Check Minmax public class, there is a problem... (none of utility ratings are better for respective executed moves... lol");
            }

            return nextMove;
        }

        private static Move maxValue(Board board, int depth)
        {
            if (board.isEnd() || depth >= MAX_DEPTH)
            {
                Move move = board.getLastMove();
                if (move == null)
                {
                    move = new Move((Board)board.DeepClone(), BoardSide.MAX);
                }

                move.setRating(UtilityRating.getRating(board));
                return move;
            }

            Move value = new Move(board, BoardSide.MAX);
            foreach (Board possibleBoard in board.getPossibleBoards())
            {
                Move currentMove = minValue(possibleBoard, depth + 1);
                value = currentMove.getRating() >= value.getRating() ? currentMove : value;
            }

            return value;
        }

        private static Move minValue(Board board, int depth)
        {
            if (board.isEnd() || depth >= MAX_DEPTH)
            {
                Move move = board.getLastMove();
                if (move == null)
                {
                    move = new Move((Board)board.DeepClone(), BoardSide.MAX);
                }

                move.setRating(UtilityRating.getRating(board));
                return move;
            }

            Move value = new Move(board, BoardSide.MIN);
            foreach (Board possibleBoard in board.getPossibleBoards())
            {
                Move currentMove = maxValue(possibleBoard, depth + 1);
                value = currentMove.getRating() <= value.getRating() ? currentMove : value;
            }

            return value;
        }
    }
}
