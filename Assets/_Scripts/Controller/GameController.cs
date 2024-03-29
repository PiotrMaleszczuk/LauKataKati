﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private App app;

    //Controllers inits
    void Awake(){
        app = App.Instance;
        app.controller.gameMode.Init ();
		app.controller.board.Init ();
		app.controller.click.Init ();
		app.controller.logic.Init ();
		app.controller.glow.Init ();
		app.controller.pause.Init ();
        app.controller.ai.Init();
		app.controller.gameOver.Init ();
		app.controller.turns.Init ();
        app.controller.bluetooth.Init ();
	}
}
