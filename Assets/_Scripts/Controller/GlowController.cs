using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowController : MonoBehaviour
{

	private App app;
	private int[][] board;
	private readonly Color GREEN = new Color (0f, 1f, 0f, 1f);
	private readonly Color BLUE = new Color (0f, 0f, 1f, 1f);
	private readonly Color HIDDEN = new Color (0f, 0f, 0f, 0f);
	private Transform boardTransform;
    
	private GameObject[] glowsArray;

	public void Init ()
	{
		app = App.Instance;
		boardTransform = app.view.board;
		board = new int[3][];
		glowsArray = app.controller.board.glowsArray;

		string boardString = "";
		for (int i = 0; i < board.Length; i++) {
			board [i] = new int[7];
			for (int j = 0; j < board [i].Length; j++) {
				if (i % 2 == 0 && j == 3) {
					board [i] [j] = -1;
					boardString += board [i] [j];
				} else {
					board [i] [j] = 0;
					boardString += board [i] [j];
				}
				boardString += " ";
			}
			boardString += "\n";
		}
		print (boardString);
	}

	public void Glowing (PawnScript pS, List<int[]> moves_list)
	{
		List<int[]> moves;
		moves = new List<int[]> ();
		for (int i = 0; i < moves_list.Count; i++) {
			moves.Add (moves_list [i]);
		}

		int iter = 0;

		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				if (board [i] [j] == -1) {
					continue;
				}

				if (pS.matrix_x == i && pS.matrix_y == j) {
					glowsArray [iter].GetComponent<SpriteRenderer> ().color = BLUE;
					iter++;
					continue;
				}

				for (int k = 0; k < moves.Count; k++) {
					if (moves [k] [0] == i && moves [k] [1] == j) {
						glowsArray [iter].GetComponent<SpriteRenderer> ().color = GREEN;
						moves.RemoveAt (k);
						break;
					}
				}
				iter++;
			}
		}
	}

	public void DisableGlows ()
	{
		for (int i = 0; i < glowsArray.Length; i++) {
			glowsArray [i].GetComponent<SpriteRenderer> ().color = HIDDEN;
		}
	}
}
