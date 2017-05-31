using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour
{

	private App app;
	private BoardController board;
    private GameModeController gameMode;

    private bool isInit = false;

	public void Init ()
	{
		app = App.Instance;
		board = app.controller.board;
        gameMode = app.controller.gameMode;

        isInit = true;
	}

	void Update ()
	{
		if (isInit) {
            if (gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_server && app.controller.turns.Turn != 1)
                return;
            if (gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_client && app.controller.turns.Turn != 2)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null)
                {
                    PawnScript pS = hit.collider.GetComponent<PawnScript>();
                    Debug.Log("Mouse");
                    board.Click(pS);
                }
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.GetTouch(0).position);
				mousePos.z = 0;

				RaycastHit2D hit = Physics2D.Raycast (mousePos, Vector2.zero);
				if (hit.collider != null) {
					PawnScript pS = hit.collider.GetComponent<PawnScript> ();
                    Debug.Log("Touch");
					board.Click (pS);
				}
			}
			
		}
	}
}
