using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    private App app;
    private GameObject pawn1;
    private GameObject pawn2;

    private int[][] board;
    private Transform[] pointArray;
    private Transform points;
    private GameObject[] pawnsArray;
    private Transform boardTransform;

    public void Init() {
        app = App.Instance;
        board = new int[3][];
        boardTransform = app.view.board;
        pawn1 = app.model.pawn1;
        pawn2 = app.model.pawn2;
        points = app.view.points;
        string boardString = "";
        pointArray = new Transform[points.childCount];
        pawnsArray = new GameObject[points.childCount-1];

        for(int i=0; i<points.childCount; i++) {
            pointArray[i] = points.GetChild(i);
        }

        int iterPawn = 0;
        int iterPos = 0;
        for (int i = 0; i < board.Length; i++) {
            board[i] = new int[7];
            for (int j = 0; j < board[i].Length; j++) {
                if (i == 1 && j == 3) {
                    board[i][j] = 0;
                    boardString += board[i][j];
                    iterPos++;
                } else {
                    if (i % 2 == 0 && j == 3) {
                        board[i][j] = -1;
                        boardString += board[i][j];
                    } else {
                        if (j <= 2) {
                            board[i][j] = 1;
                            boardString += board[i][j];
                            GameObject pawn = Instantiate(pawn1);
                            pawn.name = "Pawn " + iterPawn;
                            pawn.transform.SetParent(boardTransform);
                            pawn.transform.localPosition = new Vector3(pointArray[iterPos].localPosition.x, pointArray[iterPos].localPosition.y, 0f);
                            pawn.AddComponent<CircleCollider2D>();
                            pawnsArray[iterPawn] = pawn;
                            PawnScript pawnScript = pawnsArray[iterPawn].GetComponent<PawnScript>();
                            pawnScript.id = iterPawn;
                            pawnScript.team = 1;
                            pawnScript.matrix_x = i;
                            pawnScript.matrix_y = j;
                            iterPawn++;
                            iterPos++;
                        } else {
                            board[i][j] = 2;
                            boardString += board[i][j];
                            GameObject pawn = Instantiate(pawn2);
                            pawn.name = "Pawn " + iterPawn;
                            pawn.transform.SetParent(boardTransform);
                            pawn.transform.localPosition = new Vector3(pointArray[iterPos].localPosition.x, pointArray[iterPos].localPosition.y, 0f);
                            pawn.AddComponent<CircleCollider2D>();
                            pawnsArray[iterPawn] = pawn;
                            PawnScript pawnScript = pawnsArray[iterPawn].GetComponent<PawnScript>();
                            pawnScript.id = iterPawn;
                            pawnScript.team = 2;
                            pawnScript.matrix_x = i;
                            pawnScript.matrix_y = j;
                            iterPawn++;
                            iterPos++;
                        }
                    }
                }
                boardString += " ";
            }
            boardString += "\n";
        }
        print(boardString);
    }

    public void Click(int team, int matrix_x, int matrix_y) {
        print("team: " + team + " x: " + matrix_x + " y: " + matrix_y);
		List<int[]> tmp = new List<int[]> ();
		tmp = app.controller.logic.checking (team, matrix_x, matrix_y, board, false);
		print ("Przemko");
		for(int i=0; i<tmp.Count;i++) {
			print ("x: " + tmp[i][0] + " y: " +tmp[i][1]);
		}
		print ("Przemko end");

    }
}
