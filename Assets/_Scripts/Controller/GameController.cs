using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private App app;
    private int turn;
    private bool captureAvailable = false;
    public int GetTurn()
    {
        return turn;
    }

    public bool GetCaptureAvailable()
    {
        return captureAvailable;
    }

    //Controllers inits
    void Awake(){
        app = App.Instance;
        app.controller.board.Init();
        app.controller.click.Init();
		app.controller.logic.Init();
        app.controller.glow.Init();
        turn = Random.Range(1, 3);
        app.model.turnText.text = "Turn: Player "+turn;
	}

    public void ChangeTurn()
    {
        if (turn == 1)
            turn = 2;
        else
            turn = 1;
        app.model.turnText.text = "Turn: Player " + turn;

        GameObject[] tmpPawnsArray = app.controller.board.GetPawnsArray();
        captureAvailable = false;
        for (int i = 0; i < tmpPawnsArray.Length; i++) {
            PawnScript tmpPawnScript;
            tmpPawnScript = tmpPawnsArray[i].GetComponent<PawnScript>();


            tmpPawnScript.canCapture = false;
            if (tmpPawnScript.team != turn || tmpPawnScript.matrix_x==-1)
                continue;
            app.controller.logic.checking(turn, tmpPawnScript.matrix_x,tmpPawnScript.matrix_y, app.controller.board.GetBoard());
            if (app.controller.logic.capture)
            {
                tmpPawnScript.canCapture = true;
                captureAvailable = true;
            }
            app.controller.logic.capture = false;
        }
    }
}
