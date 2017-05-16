using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnController : MonoBehaviour {

    private App app;
    private int turn;
    private bool captureAvailable = false;
	private Text turnText;

    public int Turn
    {
		get { return turn; }
    }

    public bool CaptureAvailable
    {
		get { return captureAvailable; }
    }

    public void Init()
    {
        app = App.Instance;
		turnText = app.view.turnText;
		turn = Random.Range(1,3);
        turnText.text = "Turn: Player " + turn + "\n\nScore: 0 | 0";
		if (turn == 2)
			app.controller.ai.ComputerMakeMove (9);
    }

    public void ChangeTurn()
    {
        if (turn == 1)
            turn = 2;
        else
            turn = 1;
		CountPoints ();

        GameObject[] tmpPawnsArray = app.controller.board.PawnsArray;
        captureAvailable = false;

        for (int i = 0; i < tmpPawnsArray.Length; i++)
        {
            PawnScript tmpPawnScript;
            tmpPawnScript = tmpPawnsArray[i].GetComponent<PawnScript>();

            tmpPawnScript.canCapture = false;
            if (tmpPawnScript.matrix_x == -1)
            {
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
		if (turn == 2)
			app.controller.ai.ComputerMakeMove (9);
        
        
        
        
    }

	private void CountPoints()
	{
		int[] points = { 9, 9 };
		int[][] board = app.controller.board.Board;
		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				if (board [i] [j] == 1)
					points [1]--;
				else if (board [i] [j] == 2)
					points [0]--;
			}
		}
		if (points[0] == 9)
		{
			app.controller.gameOver.GameOver (true);
			turnText.text = "Player 1 wins!!!\n\nScore: " + points[1] + " | " + points[0];
			turn = -1;
		}
		else if (points[1] == 9)
		{
			app.controller.gameOver.GameOver (false);
			turnText.text = "Player 2 wins!!!\n\nScore: " + points[1] + " | " + points[0];
			turn = -1;
		}
		else
			turnText.text = "Turn: Player " + turn + "\n\nScore: " + points[1] + " | " + points[0];
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
