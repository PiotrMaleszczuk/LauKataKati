using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

	private App app;
	private int[][] board;

	private GameObject pawn1;
	private GameObject pawn2;
	private GameObject empty;
	private GameObject glow;

	private Transform[] pointArray;
	private Transform points;
	private Transform boardTransform;
	private Transform emptyTransform;
	private Transform glowTransform;

	private GameObject[] pawnsArray;
	private GameObject[] emptyArray;
	[HideInInspector]
	public GameObject[] glowsArray;

	private GlowController glowController;

	private PawnScript ps_chosen;
	private List<int[]> moves_list;

    public GameObject[] GetPawnsArray()
    {
        return pawnsArray;
    }
    public int[][] GetBoard()
    {
        return board;
    }

    public void Init ()
	{
		app = App.Instance;
		board = new int[3][];
		boardTransform = app.view.board;
		emptyTransform = app.view.empty;
		glowTransform = app.view.glow;
		glowController = app.controller.glow;
		glow = app.model.glows;
		pawn1 = app.model.pawn1;
		pawn2 = app.model.pawn2;
		empty = app.model.empty;
		points = app.view.points;
		string boardString = "";
		pointArray = new Transform[points.childCount];
		pawnsArray = new GameObject[points.childCount-1];
		emptyArray = new GameObject[points.childCount];
		glowsArray = new GameObject[points.childCount];

		for (int i = 0; i < points.childCount; i++) {
			pointArray [i] = points.GetChild (i);
		}

		int iterPawn = 0;
		int iterPos = 0;
		for (int i = 0; i < board.Length; i++) {
			board [i] = new int[7];
			for (int j = 0; j < board [i].Length; j++) {
				GameObject empty_position = Instantiate (empty);
				empty_position.name = "Empty " + iterPos;
				empty_position.transform.SetParent (emptyTransform);
				empty_position.transform.localPosition = new Vector3 (pointArray [iterPos].localPosition.x, pointArray [iterPos].localPosition.y, 1);
				empty_position.AddComponent<CircleCollider2D> ();
				empty_position.GetComponent<CircleCollider2D> ().radius = 1.25f;
				emptyArray [iterPos] = empty_position;
				PawnScript emptyScript = emptyArray [iterPos].GetComponent<PawnScript> ();
				emptyScript.id = iterPos;
				emptyScript.team = 0;
				emptyScript.matrix_x = i;
				emptyScript.matrix_y = j;

				ps_chosen = null;
				moves_list = new List<int[]> ();

				GameObject glowPosition = Instantiate (glow);
				glowPosition.name = "Glow " + iterPos;
				glowPosition.transform.SetParent (glowTransform);
				glowPosition.transform.localPosition = new Vector3 (pointArray [iterPos].localPosition.x, pointArray [iterPos].localPosition.y, 0f);
				glowsArray [iterPos] = glowPosition;
				SpriteRenderer renderer = glowsArray [iterPos].GetComponent<SpriteRenderer> ();
				renderer.color = new Color (0f, 1f, 0f, 0f);

				if (i == 1 && j == 3) {
					board [i] [j] = 0;
					boardString += board [i] [j];
					iterPos++;
				} else if (i % 2 == 0 && j == 3) {
					board [i] [j] = -1;
					boardString += board [i] [j];
				} else {
					if (j <= 2) {
						board [i] [j] = 1;
						boardString += board [i] [j];
						GameObject pawn = Instantiate (pawn1);
						pawn.name = "Pawn " + iterPawn;
						pawn.transform.SetParent (boardTransform);
						pawn.transform.localPosition = new Vector3 (pointArray [iterPos].localPosition.x, pointArray [iterPos].localPosition.y, 0f);
						pawn.AddComponent<CircleCollider2D> ();
						pawnsArray [iterPawn] = pawn;
						PawnScript pawnScript = pawnsArray [iterPawn].GetComponent<PawnScript> ();
						pawnScript.id = iterPawn;
						pawnScript.team = 1;
						pawnScript.matrix_x = i;
						pawnScript.matrix_y = j;
						iterPawn++;
						iterPos++;
					} else {
						board [i] [j] = 2;
						boardString += board [i] [j];
						GameObject pawn = Instantiate (pawn2);
						pawn.name = "Pawn " + iterPawn;
						pawn.transform.SetParent (boardTransform);
						pawn.transform.localPosition = new Vector3 (pointArray [iterPos].localPosition.x, pointArray [iterPos].localPosition.y, 0f);
						pawn.AddComponent<CircleCollider2D> ();
						pawnsArray [iterPawn] = pawn;
						PawnScript pawnScript = pawnsArray [iterPawn].GetComponent<PawnScript> ();
						pawnScript.id = iterPawn;
						pawnScript.team = 2;
						pawnScript.matrix_x = i;
						pawnScript.matrix_y = j;
						iterPawn++;
						iterPos++;
					}

				}
				boardString += " ";
			}
			boardString += "\n";
		}
		print (boardString);
	}

	public void Click (PawnScript pawnScript)
	{
		print ("Clicked - team: " + pawnScript.team + " x: " + pawnScript.matrix_x + " y: " + pawnScript.matrix_y);
        if (ps_chosen == null && pawnScript.team == app.controller.turns.GetTurn())
        {
            if (app.controller.turns.GetCaptureAvailable() != pawnScript.canCapture) {
                print("CaptureAvailable!");
                return;
            }
            ps_chosen = pawnScript;
            print("Chosen pawn: " + ps_chosen.id);
            moves_list = app.controller.logic.checking(pawnScript.team, pawnScript.matrix_x, pawnScript.matrix_y, board);
            for (int i = 0; i < moves_list.Count; i++)
            {
                print("Logic = x: " + moves_list[i][0] + " y: " + moves_list[i][1]);
            }
            glowController.Glowing(ps_chosen, moves_list);
        }
        else if (ps_chosen == pawnScript)
        {
            print("Unchosen pawn: " + pawnScript.id);
            ps_chosen = null;
            app.controller.logic.capture = false;
            moves_list.Clear();
            glowController.DisableGlows();
        }
        else if (pawnScript.team == 0 && ps_chosen != null)
        {
            for (int i = 0; i < moves_list.Count; i++)
            {
                if (pawnScript.matrix_x == moves_list[i][0] && pawnScript.matrix_y == moves_list[i][1])
                {
                    if (app.controller.logic.capture)
                    {
                        int captured_x;
                        if (ps_chosen.matrix_x == 1)
                            captured_x = pawnScript.matrix_x;
                        else if (pawnScript.matrix_x == 1)
                            captured_x = ps_chosen.matrix_x;
                        else
                            captured_x = ps_chosen.matrix_x + (pawnScript.matrix_x - ps_chosen.matrix_x) / 2;
                        int captured_y = ps_chosen.matrix_y + (pawnScript.matrix_y - ps_chosen.matrix_y) / 2;
                        for (int j = 0; j < pawnsArray.Length; j++)
                        {
                            PawnScript ps_captured = pawnsArray[j].GetComponent<PawnScript>();
                            if (ps_captured.matrix_x == captured_x && ps_captured.matrix_y == captured_y)
                            {
                                ps_captured.matrix_x = -1;
                                ps_captured.matrix_y = -1;
                                ps_captured.gameObject.SetActive(false);
                                board[captured_x][captured_y] = 0;
                                break;
                            }
                        }
                        moves_list = app.controller.logic.checking(ps_chosen.team, pawnScript.matrix_x, pawnScript.matrix_y, board);
                    }
                    board[ps_chosen.matrix_x][ps_chosen.matrix_y] = 0;
                    ps_chosen.matrix_x = pawnScript.matrix_x;
                    ps_chosen.matrix_y = pawnScript.matrix_y;
                    ps_chosen.transform.localPosition = new Vector3(pawnScript.transform.localPosition.x, pawnScript.transform.localPosition.y, 0f);
                    board[pawnScript.matrix_x][pawnScript.matrix_y] = ps_chosen.team;
                    glowController.DisableGlows();
                    if (!app.controller.logic.capture)
                    {
                        moves_list.Clear();
                        ps_chosen = null;
                        app.controller.turns.ChangeTurn();

                    }
                    else
                    {
                        glowController.Glowing(ps_chosen, moves_list);
                    }
                    break;
                }
            }
        }
	}
}
