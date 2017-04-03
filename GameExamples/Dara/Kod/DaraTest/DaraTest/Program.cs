using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaraLibrary;

namespace DaraTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();

            board.putPawn(new Pawn(BoardSide.MAX));
            board.putPawn(new Pawn(BoardSide.MAX));
            board.putPawn(new Pawn(BoardSide.MAX));
            board.putPawn(new Pawn(BoardSide.MAX));
            board.putPawn(new Pawn(BoardSide.MAX));
            board.putPawn(new Pawn(BoardSide.MAX));

            board.putPawn(new Pawn(BoardSide.MIN));
            board.putPawn(new Pawn(BoardSide.MIN));
            board.putPawn(new Pawn(BoardSide.MIN));

            System.Console.WriteLine("before: ");
            board.printBoard();

            
            Move nextMove = Minmax.getNextMove(board);
            System.Console.WriteLine("after: ");
            nextMove.executeMove();
            nextMove.getBoard().printBoard();
            System.Console.ReadLine();
            /*
            board.toggleTurnOwner();
            System.Console.WriteLine("possible: ");
            board.printPossibleBoards();
            System.Console.ReadLine();*/
        }
    }
}
