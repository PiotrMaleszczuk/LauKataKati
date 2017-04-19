using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnController : MonoBehaviour {

    private App app;
    private int turn;
    private bool captureAvailable = false;
    public Text turnText;

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
        turn = Random.Range(1, 3);
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
            app.controller.logic.checking(turn, tmpPawnScript.matrix_x, tmpPawnScript.matrix_y, app.controller.board.GetBoard());
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
    }
}
