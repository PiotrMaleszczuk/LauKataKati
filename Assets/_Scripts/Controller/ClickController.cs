using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour
{

	private App app;
	private BoardController board;
	private bool isInit = false;

	public void Init ()
	{
		app = App.Instance;
		board = app.controller.board;
		isInit = true;
	}

	void Update ()
	{
		if (isInit) {
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.GetTouch(0).position);
				mousePos.z = 0;

				RaycastHit2D hit = Physics2D.Raycast (mousePos, Vector2.zero);
				if (hit.collider != null) {
					PawnScript pS = hit.collider.GetComponent<PawnScript> ();
					board.Click (pS);
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				mousePos.z = 0;

				RaycastHit2D hit = Physics2D.Raycast (mousePos, Vector2.zero);
				if (hit.collider != null) {
					PawnScript pS = hit.collider.GetComponent<PawnScript> ();
					board.Click (pS);
				}
			}
		}
	}
}
