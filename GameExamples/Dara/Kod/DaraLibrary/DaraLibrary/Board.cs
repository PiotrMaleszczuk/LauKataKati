using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    [Serializable]
    public class Board
    {
        public static int BOARD_COLUMN_COUNT = 5;
        public static int BOARD_ROW_COUNT = 6;
        public static bool delete =false;

        private static Random rand = new Random(); 

        private BoardSquare[][] boardSquares;
        private BoardSide turnOwner;
        private Move lastMove;

        public Board() 
        {
            boardSquares = new BoardSquare[BOARD_ROW_COUNT][];
            turnOwner = BoardSide.MAX;

            for (int i = 0; i < boardSquares.Length; i++)
            {
                boardSquares[i] = new BoardSquare[BOARD_COLUMN_COUNT];
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    boardSquares[i][j] = new BoardSquare(i, j);
                }
            }
        }

        public int putPawn(BoardSquare where, Pawn pawn)
        {
            BoardSquare square = boardSquares[where.getColumnCoord()][where.getRowCoord()];
            int returned =square.setCurrentObject(pawn);
            int reutrnedfours = findFours();
            if (reutrnedfours == 1)
            {
                square = boardSquares[where.getColumnCoord()][where.getRowCoord()];
                square.takeCurrentObject();
                return 2 ;
            }
            if (returned == 2)
            {
                return 1;
                    
            }
            else
            {
                return 0;
            }
            
            //where.setCurrentObject(pawn);
            
        }
        
        // for testing purposes
        public void putPawn(Pawn pawn)
        {
            int row = rand.Next(BOARD_ROW_COUNT);
            int column = rand.Next(BOARD_COLUMN_COUNT);
            BoardSquare square = boardSquares[row][column];
            int i = square.setCurrentObject(pawn);
            while (i == 2)
            {
                 

                
                    
                    row = rand.Next(BOARD_ROW_COUNT);
                    column = rand.Next(BOARD_COLUMN_COUNT);
                    square = boardSquares[row][column];
                    i = square.setCurrentObject(pawn);
                    int reutrned = findFours();
                    if (reutrned == 1)
                    {
                        square = boardSquares[row][column];
                        square.takeCurrentObject();
                        i = 2;
                    }
                
                //findAndLockThrees();
                
            }
        }

        public BoardSide getTurnOwner()
        {
            return turnOwner;
        }

        public void toggleTurnOwner()
        {
            if (turnOwner == BoardSide.MAX)
            {
                turnOwner = BoardSide.MIN;
            }
            else if (turnOwner == BoardSide.MIN)
            {
                turnOwner = BoardSide.MAX;
            }
            else
            {
                throw new NoneTurnException("Something or someone has set turn owner to BoardObjectSide.NONE - why? :(");
            }
        }

        public void setLastMove(Move lastMove)
        {
            this.lastMove = lastMove;
        }

        public Move getLastMove()
        {
            return lastMove;
        }

        public bool isEnd()
        {
            List<BoardSquare> maxNotLockedSquares = new List<BoardSquare>();
            List<BoardSquare> minNotLockedSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX &&
                        !boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        maxNotLockedSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN &&
                        !boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        minNotLockedSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            if (maxNotLockedSquares.Count < 3 || minNotLockedSquares.Count < 3)
            {
                return true;
            }

            return false;
        }

        public int isEnd(int cos)
        {
            List<BoardSquare> maxNotLockedSquares = new List<BoardSquare>();
            List<BoardSquare> minNotLockedSquares = new List<BoardSquare>();
            int win = 0;
            int lose = 0;
            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX &&
                        !boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        maxNotLockedSquares.Add(boardSquares[i][j]);
                    }
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX &&
                        boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        win += 1;
                    }
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN &&
                        !boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        minNotLockedSquares.Add(boardSquares[i][j]);
                    }
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN &&
                        boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        lose += 1;
                    }
                }
            }

            if (maxNotLockedSquares.Count < 3 || minNotLockedSquares.Count < 3)
            {
                if (win > lose)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
            return 0;
        }
        public List<Board> getPossibleBoards()
        {
            List<BoardSquare> freeSquares = new List<BoardSquare>();
            List<BoardSquare> turnOwnerSquares = new List<BoardSquare>();
            List<Board> possibleBoards = new List<Board>();
            List<Move> validMoves = new List<Move>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is None)
                    {
                        freeSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn && 
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == turnOwner)
                    {
                        turnOwnerSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare from in turnOwnerSquares)
            {
                foreach (BoardSquare to in freeSquares)
                {
                    Board boardClone = (Board)this.DeepClone();
                    BoardSquare fromClone = boardClone.getBoardSquare(from.getRowCoord(), from.getColumnCoord());
                    BoardSquare toClone = boardClone.getBoardSquare(to.getRowCoord(), to.getColumnCoord());

                    Move move = new Move(boardClone, fromClone, toClone);
                    if (move.isValid())
                    {
                        validMoves.Add(move);
                    }
                }
            }

            foreach (Move move in validMoves)
            {
                try
                {
                    move.executeMove();
                    possibleBoards.Add(move.getBoard());
                }
                catch (InvalidMoveExecutionException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }

            return possibleBoards;
        }

        public int findFours()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                int returned=returnFours(maxSquare);
                if(returned!=0)
                    return returned;
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                int returned=returnFours(minSquare);
                if (returned != 0)
                    return returned;
            }
            return 0;
        }

        private int returnFours(BoardSquare square)
        {
            if (square.getCurrentObject().getLocked())
            {
                return 0;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                BoardSquare upperNeighbour1 = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour2 = boardSquares[rowCoords + 2][columnCoords];
                BoardSquare upperNeighbour3 = boardSquares[rowCoords + 3][columnCoords];

                if (upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour2.getCurrentObject() is Pawn &&
                    upperNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour3.getCurrentObject() is Pawn &&
                    upperNeighbour3.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    return 1;

                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour2 = boardSquares[rowCoords - 2][columnCoords];
                BoardSquare lowerNeighbour3 = boardSquares[rowCoords - 3][columnCoords];

                if (lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour2.getCurrentObject() is Pawn &&
                    lowerNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour3.getCurrentObject() is Pawn &&
                    lowerNeighbour3.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    return 1;

                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare upperNeighbour1 = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour2 = boardSquares[rowCoords + 2][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour2 = boardSquares[rowCoords - 2][columnCoords];

                if (upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour2.getCurrentObject() is Pawn &&
                    lowerNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() )
                {
                    return 1;
                    
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            
            try
            {
                BoardSquare upperNeighbour1 = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour2 = boardSquares[rowCoords + 2][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour2 = boardSquares[rowCoords - 2][columnCoords];

                if (upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour2.getCurrentObject() is Pawn &&
                    upperNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    return 1;

                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }

            try
            {
                BoardSquare leftNeighbour1 = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour2 = boardSquares[rowCoords][columnCoords - 2];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour2 = boardSquares[rowCoords][columnCoords + 2];

                if (leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour2.getCurrentObject() is Pawn &&
                    leftNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() )
                {

                    return 1;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour1 = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour2 = boardSquares[rowCoords][columnCoords - 2];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour2 = boardSquares[rowCoords][columnCoords + 2];

                if (leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour2.getCurrentObject() is Pawn &&
                    rightNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {

                    return 1;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }

            try
            {
                BoardSquare leftNeighbour1 = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour2 = boardSquares[rowCoords][columnCoords - 2];
                BoardSquare leftNeighbour3 = boardSquares[rowCoords][columnCoords - 3];

                if (leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour2.getCurrentObject() is Pawn &&
                    leftNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour3.getCurrentObject() is Pawn &&
                    leftNeighbour3.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    return 1;

                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare rightNeighbour1 = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour2 = boardSquares[rowCoords][columnCoords + 2];
                BoardSquare rightNeighbour3 = boardSquares[rowCoords][columnCoords + 3];

                if (rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour2.getCurrentObject() is Pawn &&
                    rightNeighbour2.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour3.getCurrentObject() is Pawn &&
                    rightNeighbour3.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    return 1;

                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public void findAndLockThrees()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                lockThrees(maxSquare);
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                lockThrees(minSquare);
            }
        }
        public void isThrees(BoardSquare position)
        {
            lockThrees(position);
            
        }

        private void lockThrees(BoardSquare square)
        {
            if (square.getCurrentObject().getLocked())
            {
                return ;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour1 = boardSquares[rowCoords + 2][columnCoords];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    square.getCurrentObject().setLocked(true);
                    upperNeighbour.getCurrentObject().setLocked(true);
                    upperNeighbour1.getCurrentObject().setLocked(true);
                    delete = true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 2][columnCoords];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    square.getCurrentObject().setLocked(true);
                    lowerNeighbour.getCurrentObject().setLocked(true);
                    lowerNeighbour1.getCurrentObject().setLocked(true);
                    delete = true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    square.getCurrentObject().setLocked(true);
                    upperNeighbour.getCurrentObject().setLocked(true);
                    lowerNeighbour.getCurrentObject().setLocked(true);
                    delete = true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour1 = boardSquares[rowCoords][columnCoords - 2];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    square.getCurrentObject().setLocked(true);
                    leftNeighbour.getCurrentObject().setLocked(true);
                    leftNeighbour1.getCurrentObject().setLocked(true);
                    delete = true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            } try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords +1];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords][columnCoords +2];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    square.getCurrentObject().setLocked(true);
                    rightNeighbour.getCurrentObject().setLocked(true);
                    rightNeighbour1.getCurrentObject().setLocked(true);
                    delete = true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    square.getCurrentObject().setLocked(true);
                    leftNeighbour.getCurrentObject().setLocked(true);
                    rightNeighbour.getCurrentObject().setLocked(true);
                    delete = true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            
        }

        public int getMaxSidePawnCount()
        {
            int counter = 0;
            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }





        public int getMinSidePawnCount()
        {
            int counter = 0;
            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        public BoardSquare getBoardSquare(int row, int column)
        {
            return boardSquares[row][column];
        }

        // printing
        

        public Char[,] printBoard()
        {
            StringBuilder sb = new StringBuilder();
            Char[,] array = new Char[6,5];
            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is None)
                    {
                        array[i,j] = '_';
                        sb.Append("_");
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        if (boardSquares[i][j].getCurrentObject().getLocked())
                        {
                            array[i, j] = 'M';
                            sb.Append("M");
                        }
                        else
                        {
                            array[i, j] = 'm';
                            sb.Append("m");
                        }
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        if (boardSquares[i][j].getCurrentObject().getLocked())
                        {
                            array[i, j] = 'I';
                            sb.Append("I");
                        }
                        else
                        {
                            array[i, j] = 'i';
                            sb.Append("i");
                        }
                    }
                }
                sb.AppendLine();
            }
            return array;
           // System.Console.WriteLine(sb.ToString());
        }

        public void printPossibleBoards()
        {
            List<Board> possibleBoards = getPossibleBoards();
            int i = 0;
            foreach (Board board in possibleBoards)
            {
                System.Console.WriteLine(i);
                board.printBoard();
                i++;
            }
        }

        public void findAndRateTwos()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                FindTwos(maxSquare);
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                FindTwos(minSquare);
            }
        }



        private void FindTwos(BoardSquare square)
        {
            
            if (square.getCurrentObject().getLocked())
            {
                return;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 2;
                    //set rating
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() )
                {
                    UtilityRating.rating += 2;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() )
                {
                    UtilityRating.rating += 2;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            } try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() )
                {
                    UtilityRating.rating += 2;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            
        }



        public void findAndRateCloseToTwos()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                FindCloseToTwos(maxSquare);
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                FindCloseToTwos(minSquare);
            }
        }



        private void FindCloseToTwos(BoardSquare square)
        {
            if (square.getCurrentObject().getLocked())
            {
                return;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords+1];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {

                    UtilityRating.rating += 1;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords+1];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 1;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords-1][columnCoords - 1];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 1;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            } try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords+1][columnCoords -1];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 1;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }


        }


        public void findAndRateCloseToThrees()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                FindCloseToThrees(maxSquare);
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                FindCloseToThrees(minSquare);
            }
        }




        private void FindCloseToThrees(BoardSquare square)
        {
            if (square.getCurrentObject().getLocked())
            {
                return;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour1 = boardSquares[rowCoords +2][columnCoords+1];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour1 = boardSquares[rowCoords + 2][columnCoords- 1];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour1 = boardSquares[rowCoords +3][columnCoords];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords -2][columnCoords+1];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 2][columnCoords -1];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 3][columnCoords];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour1 = boardSquares[rowCoords-1][columnCoords - 2];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }

            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour1 = boardSquares[rowCoords + 1][columnCoords - 2];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour1 = boardSquares[rowCoords][columnCoords - 3];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords+1][columnCoords + 2];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords - 1][columnCoords + 2];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords][columnCoords + 3];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 3;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            
        }






        public void findAndRateCloseMoves()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                FindCloseToMoves(maxSquare);
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                FindCloseToMoves(minSquare);
            }
        }




        private void FindCloseToMoves(BoardSquare square)
        {
            if (square.getCurrentObject().getLocked())
            {
                return;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        BoardSquare upperNeighbour = boardSquares[rowCoords + i][columnCoords+j];


                        if (upperNeighbour.getCurrentObject() is Pawn &&
                            upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                        {
                            double sum =  1/(i +  j+0.001) ;
                            UtilityRating.rating += sum;
                        }
                    }
                }
                
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            

        }









        public void findAndRateThrees()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX)
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            foreach (BoardSquare maxSquare in maxSquares)
            {
                RateThrees(maxSquare);
            }

            foreach (BoardSquare minSquare in minSquares)
            {
                RateThrees(minSquare);
            }
        }
        

        private void RateThrees(BoardSquare square)
        {
            if (square.getCurrentObject().getLocked())
            {
                return;
            }

            int rowCoords = square.getRowCoord();
            int columnCoords = square.getColumnCoord();
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare upperNeighbour1 = boardSquares[rowCoords + 2][columnCoords];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    upperNeighbour1.getCurrentObject() is Pawn &&
                    upperNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 4;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];
                BoardSquare lowerNeighbour1 = boardSquares[rowCoords - 2][columnCoords];

                if (lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour1.getCurrentObject() is Pawn &&
                    lowerNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 4;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare upperNeighbour = boardSquares[rowCoords + 1][columnCoords];
                BoardSquare lowerNeighbour = boardSquares[rowCoords - 1][columnCoords];

                if (upperNeighbour.getCurrentObject() is Pawn &&
                    upperNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    lowerNeighbour.getCurrentObject() is Pawn &&
                    lowerNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 4;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare leftNeighbour1 = boardSquares[rowCoords][columnCoords - 2];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    leftNeighbour1.getCurrentObject() is Pawn &&
                    leftNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 4;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            } try
            {
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];
                BoardSquare rightNeighbour1 = boardSquares[rowCoords][columnCoords + 2];

                if (rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour1.getCurrentObject() is Pawn &&
                    rightNeighbour1.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 4;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
            try
            {
                BoardSquare leftNeighbour = boardSquares[rowCoords][columnCoords - 1];
                BoardSquare rightNeighbour = boardSquares[rowCoords][columnCoords + 1];

                if (leftNeighbour.getCurrentObject() is Pawn &&
                    leftNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide() &&
                    rightNeighbour.getCurrentObject() is Pawn &&
                    rightNeighbour.getCurrentObject().getBoardObjectSide() == square.getCurrentObject().getBoardObjectSide())
                {
                    UtilityRating.rating += 4;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine(ex.Message);
            }
        }


        


        public List<BoardSquare> findAllMax()
        {
            List<BoardSquare> maxSquares = new List<BoardSquare>();
            List<BoardSquare> minSquares = new List<BoardSquare>();

            for (int i = 0; i < boardSquares.Length; i++)
            {
                for (int j = 0; j < boardSquares[i].Length; j++)
                {
                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MAX && !boardSquares[i][j].getCurrentObject().getLocked())
                    {
                        maxSquares.Add(boardSquares[i][j]);
                    }

                    if (boardSquares[i][j].getCurrentObject() is Pawn &&
                        boardSquares[i][j].getCurrentObject().getBoardObjectSide() == BoardSide.MIN)
                    {
                        minSquares.Add(boardSquares[i][j]);
                    }
                }
            }

            return maxSquares;
        }
    }
}
