using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    private App app;
    private Transform boardTransform;

    private int[][] board;

    public void Init() {
        app = App.Instance;
        boardTransform = app.view.board.transform;
        board = new int[3][];
        string boardString = "";

        for (int i = 0; i < board.Length; i++) {
            board[i] = new int[7];
            for (int j = 0; j < board[i].Length; j++) {
                if (i == 1 && j == 3) {
                    board[i][j] = 0;
                    boardString += board[i][j];
                } else {
                    if (i % 2 == 0 && j == 3) {
                        board[i][j] = -1;
                        boardString += board[i][j];
                    } else {
                        if (j <= 2) {
                            board[i][j] = 1;
                            boardString += board[i][j];
                        } else {
                            board[i][j] = 2;
                            boardString += board[i][j];
                        }
                    }
                }
                boardString += " ";
            }
            boardString += "\n";
        }
        print(boardString);
    }
}
