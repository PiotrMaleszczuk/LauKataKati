using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnController : MonoBehaviour {

    private App app;
    private int turn;
    private bool captureAvailable = false;
	private Text turnText;

    public int GetTurn()
    {
        return turn;
    }

    public bool GetCaptureAvailable()
    {
        return captureAvailable;
    }

    public void Init()
    {
        app = App.Instance;
		turnText = app.view.turnText;
        turn = 1;
        turnText.text = "Turn: Player " + turn + "\n\nScore: 0 | 0";
    }

    public void ChangeTurn()
    {
        int[] points = { 0, 0 };
        if (turn == 1)
            turn = 2;
        else
            turn = 1;

        GameObject[] tmpPawnsArray = app.controller.board.GetPawnsArray();
        captureAvailable = false;

        for (int i = 0; i < tmpPawnsArray.Length; i++)
        {
            PawnScript tmpPawnScript;
            tmpPawnScript = tmpPawnsArray[i].GetComponent<PawnScript>();

            tmpPawnScript.canCapture = false;
            if (tmpPawnScript.matrix_x == -1)
            {
                points[tmpPawnScript.team - 1]++;
                continue;
            }
            if (tmpPawnScript.team != turn)
                continue;
            app.controller.logic.checking(turn, tmpPawnScript.matrix_x, tmpPawnScript.matrix_y, app.controller.board.Board);
            if (app.controller.logic.capture)
            {
                tmpPawnScript.canCapture = true;
                captureAvailable = true;
            }
            app.controller.logic.capture = false;
        }
        
        if (points[0] == 9)
        {
            turnText.text = "Player 1 wins!!!\n\nScore: " + points[1] + " | " + points[0];
            turn = -1;
        }
        else if (points[1] == 9)
        {
            turnText.text = "Player 2 wins!!!\n\nScore: " + points[1] + " | " + points[0];
            turn = -1;
        }
        else
            turnText.text = "Turn: Player " + turn + "\n\nScore: " + points[1] + " | " + points[0];
        
        if (turn == 2)
        {
            print("before");
            PrintBoard(app.controller.board.Board);
            int[][] board_before = app.controller.board.Board;
            int[][] board_after = app.controller.ai.ComputerMakeMove(9);
            for(int i=0;i<board_after.Length;i++)
            {
                for(int j=0;j<board_after[i].Length;j++)
                {
                    if (board_before[i][j] == 2 && board_after[i][j]==0)
                    {
                        for (int k = 0; k < tmpPawnsArray.Length; k++)
                        {
                            PawnScript tmpPawnScript;
                            tmpPawnScript = tmpPawnsArray[k].GetComponent<PawnScript>();
                            if (tmpPawnScript.matrix_x==i && tmpPawnScript.matrix_y==j)
                            {
                                app.controller.board.Click(tmpPawnScript);
                            }
                        }
                    }
                }
            }
            GameObject[] tmpEmptyArray = app.controller.board.EmptyArray;
            for (int i = 0; i < board_after.Length; i++)
            {
                for (int j = 0; j < board_after[i].Length; j++)
                {
                    if (board_before[i][j] == 0 && board_after[i][j] == 2)
                    {
                        for (int k = 0; k < tmpEmptyArray.Length; k++)
                        {
                            PawnScript tmpPawnScript;
                            tmpPawnScript = tmpEmptyArray[k].GetComponent<PawnScript>();
                            if (tmpPawnScript.matrix_x == i && tmpPawnScript.matrix_y == j)
                            {
                                print(tmpPawnScript.id);
                                app.controller.board.Click(tmpPawnScript);
                            }
                        }
                    }
                }
            }
            print("after");
            PrintBoard(app.controller.board.Board);
            //ChangeTurn();
        }
    }
    private void PrintBoard(int[][] tab)
    {
        string boardString = "";
        for (int i = 0; i < tab.Length; i++)
        {
            for (int j = 0; j < tab[i].Length; j++)
            {
                boardString += tab[i][j];
                boardString += " ";
            }
            boardString += "\n";
        }
        print(boardString);
    }

}
