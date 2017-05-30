using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnController : MonoBehaviour {

    private App app;
    private int turn;
    private bool captureAvailable = false;
	private Text turnText;
	private int turnsWithoutCapture;
	private int[] points;
    private Mode mode;
 
    private enum Mode
    {
        single,
        multiplayer_local,
        multiplayer_bluetooth_client,
        multiplayer_bluetooth_server
    }

    public int TurnsWithouCapture 
	{
		get { return turnsWithoutCapture; }
	}
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
        int modeIndex = SaveDataController.Instance.Data.mode;
        switch (modeIndex)
        {
            case 1:
                this.mode = Mode.single;
                break;
            case 2:
                this.mode = Mode.multiplayer_local;
                break;
            case 3:
                this.mode = Mode.multiplayer_bluetooth_server;
                break;
            case 4:
                this.mode = Mode.multiplayer_bluetooth_client;
                break;
        }
    captureAvailable = false;
		turnsWithoutCapture = 0;
		points = new int[]{ 0, 0 };
		turnText = app.view.turnText;
		turn = Random.Range(1,3);
        turnText.text = "Turn: Player " + turn + "\n\nScore: 0 | 0";
        if (mode == Mode.single)
        {
            if (turn == 2)
                app.controller.ai.ComputerMakeMove();
        }
    }

    public void ChangeTurn()
    {
        if (turn == 1)
            turn = 2;
		else if (turn == 2)
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
        if (mode == Mode.single)
        {
            if (turn == 2)
                app.controller.ai.ComputerMakeMove();
        }


    }

	private void CountPoints()
	{
		int[] tmp_points = new int[2];
		tmp_points[0] = points[0];
		tmp_points[1] = points[1];
		points = new int[]{ 9, 9 };
		int[][] board = app.controller.board.Board;
		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				if (board [i] [j] == 1)
					points [1]--;
				else if (board [i] [j] == 2)
					points [0]--;
			}
		}
		if (tmp_points [0] == points [0] && tmp_points [1] == points [1])
			turnsWithoutCapture++;
		else
			turnsWithoutCapture = 0;

		if (TurnsWithouCapture >= 20) {
			if (points [0] > points [1]) 
			{
				turnText.text = "Out of moves!\n\nPlayer 1 wins!!!\n\nScore: " + points [0] + " | " + points [1];
				turn = -1;
				app.controller.gameOver.GameOver (1);
			} 
			else if (points [1] > points [0]) 
			{
				turnText.text = "Out of moves!\n\nPlayer 2 wins!!!\n\nScore: " + points [0] + " | " + points [1];
				turn = -1;
				app.controller.gameOver.GameOver (2);
			} 
			else 
			{
				turnText.text = "Out of moves!\n\nDraw!!!\n\nScore: " + points [0] + " | " + points [1];
				turn = -1;
				app.controller.gameOver.GameOver (3);
			}
			
		}

		if (points[0] == 9)
		{
			turnText.text = "Player 1 wins!!!\n\nScore: " + points[0] + " | " + points[1];
			turn = -1;
			app.controller.gameOver.GameOver (1);
		}
		else if (points[1] == 9)
		{
			turnText.text = "Player 2 wins!!!\n\nScore: " + points[0] + " | " + points[1];
			turn = -1;
			app.controller.gameOver.GameOver (2);
		}
		else
			turnText.text = "Turn: Player " + turn + "\n\nScore: " + points[0] + " | " + points[1]
				+ "\n\nWithout capture: "+turnsWithoutCapture+"/20";
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
