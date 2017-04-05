using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicController : MonoBehaviour {
	private int[][] board;
	private bool capture;
	private int enemy_team;
	private int mtx_x, mtx_y;

	private List<int[]> capture_moves_list;
	private List<int[]> normal_moves_list;

	public void Init()
	{
		capture_moves_list = new List<int[]> ();
		normal_moves_list = new List<int[]> ();

	}
		

	public List<int[]> checking(int team, int mtx_x, int mtx_y, int[][] board, bool capture)
	{
		this.board = board;
		this.capture = capture;
		this.mtx_x = mtx_x;
		this.mtx_y = mtx_y;
		capture_moves_list.Clear ();
		normal_moves_list.Clear ();

		if (team == 1)
			this.enemy_team = 2;
		else
			this.enemy_team = 1;


		if (mtx_x == 0 && mtx_y == 2)
			extension_check (1, 1);

		if (mtx_x == 2 && mtx_y == 2)
			extension_check (1, -1);

		if (mtx_x == 0 && mtx_y == 4)
			extension_check (-1, -1);

		if (mtx_x == 2 && mtx_y == 4)
			extension_check (-1, -1);

		//dodatkowa pozycja(1,3)
		if (mtx_x == 1 && mtx_y == 3) {
			extension_check (1, 1);
			extension_check (-1, 1);
			extension_check (1, -1);
			extension_check (-1, -1);
		}


		//klasyczne sprawdzenie
		for (int i = -1; i < 2; i += 2) {
			if (mtx_x + i >= 0 && mtx_x + i <= 6) {
				extension_check (i, 0);
			}
			if (mtx_y + i >= 0 && mtx_y + i <= 2) {
				extension_check (0, i);
			}
		}
		print ("elo");

		//przydaloby sie rowniez odeslac flage "capture"
		if (this.capture) {
			print (capture_moves_list.Count);
			return capture_moves_list;
		} else {
			print (normal_moves_list.Count);
			return normal_moves_list;
		}
	}

	private void extension_check(int x_it, int y_it)
	{
		if (board [mtx_x + x_it] [mtx_y + y_it] == enemy_team) {
			if (board [mtx_x + x_it + x_it] [mtx_y + y_it + y_it] == 0) {
				//dodaj do listy mozliwych bic
				int[] tmp_array = new int[2];
				tmp_array [0] = mtx_x + x_it;
				tmp_array [1] = mtx_y + y_it;
				capture_moves_list.Add (tmp_array);
				capture = true;
			}
		} else if (!capture) {
			if (board [mtx_x + x_it] [mtx_y + y_it] == 0) {
				//dodaj do listy zwyklych ruchow
				int[] tmp_array = new int[2];
				tmp_array [0] = mtx_x + x_it;
				tmp_array [1] = mtx_y + y_it;
				normal_moves_list.Add (tmp_array);
			}
		}		

	}

}
