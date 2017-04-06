using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowController : MonoBehaviour {

    private App app;
    private int[][] board;
    private readonly Color GREEN = new Color(0f, 1f, 0f, 1f);
    private readonly Color BLUE = new Color(0f, 0f, 1f, 1f);
    private readonly Color HIDDEN = new Color(0f, 0f, 0f, 0f);
    private Transform boardTransform;
    
    private GameObject[] glowsArray;

    public void Init() {
        app = App.Instance;
        boardTransform = app.view.board;
        board = new int[3][];

        for (int i=0; i<board.Length; i++) {
            board[i] = new int[7];
            for (int j=0; j<board[i].Length; i++) {
                if (i % 2 == 0 && j == 3) {
                    board[i][j] = -1;
                } else {
                    board[i][j] = 0;
                }
            }
        }
    }

    public void Glowing(PawnScript pS, List<int[]> moves_list) {
        List<int[]> moves = moves_list;

        int iter = 0;
        for (int i=0; i<board.Length; i++) {
            for (int j=0; j<board[i].Length; j++) {
                if (board[i][j] == -1) {
                    continue;
                }

                if(pS.matrix_x==i && pS.matrix_y == j) {
                    glowsArray[iter].GetComponent<SpriteRenderer>().color = BLUE;
                    iter++;
                    continue;
                }

                for(int k=0; k<moves.Count; k++) {
                    if(moves[k][0] == i && moves[k][1] == j) {
                        glowsArray[iter].GetComponent<SpriteRenderer>().color = GREEN;
                        iter++;
                        moves.RemoveAt(k);
                        continue;
                    }
                }
            }
        }
    }

    public void DisableGlows(GameObject[] glowsArray) {
        for(int i=0; i<glowsArray.Length; i++) {
            glowsArray[i].GetComponent<SpriteRenderer>().color = HIDDEN;
        }
    }
}
