using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AIController : MonoBehaviour {
	private App app;
	struct Available
	{
		public List<int[][]> boards; //dostępne plansze
		public List<List<int[]>> moves; //dostępne ruchy
	}
	private int depth; //głębokość przeszukiwania

	public void Init() //metoda inicjalizujaca
	{
		app = App.Instance;
		depth = SaveDataController.Instance.Data.difficult;
	}

    public void ComputerMakeMove() //metoda wywoływana zewnętrznie nakazująca maszynie grającej znalezienie następnego ruchu
    {
        AI_MiniMax Current = new AI_MiniMax(app.controller.board.Board, false); //budowanie stanu maszyny na podstawie aktualnego stanu planszy
        AI_MiniMax next = Current.FindNextMove(depth); //szukanie najlepszego ruchu
        if (next != null)
        {
            ComputerClick(next.Moves); //wywołanie imitacji kliknięcia
        }
    }

    private void ComputerClick(List<int[]> moveToClick) //metoda imitująca kliknięcie na podstawie najlepszego znalezionego ruchu
	{
		GameObject[] pawnsArray = app.controller.board.PawnsArray;
		GameObject[] emptyArray = app.controller.board.EmptyArray;
		for (int i = 0; i < pawnsArray.Length; i++) {
			PawnScript tmpPawnScript = pawnsArray [i].GetComponent<PawnScript> ();
			if (tmpPawnScript.matrix_x == moveToClick [0] [0] && tmpPawnScript.matrix_y == moveToClick [0] [1]) {
				app.controller.board.Click (tmpPawnScript); //wywołanie kliknięcia pionka
			}
		}

		for (int i = 1; i < moveToClick.Count; i++) 
		{ 
			for (int j = 0; j < emptyArray.Length; j++) 
			{
				PawnScript tmpPawnScript = emptyArray [j].GetComponent<PawnScript> ();
				if (tmpPawnScript.matrix_x == moveToClick [i] [0] && tmpPawnScript.matrix_y == moveToClick [i] [1]) 
				{
					app.controller.board.Click (tmpPawnScript); //wywołanie kliknięcia pustej pozycji
				}
			}
		}
	}

   
	class AI_MiniMax //klasa odpowiadająca za rdzeń maszyny grającej
	{
		private int[][] board;
		private List<int[]> moves; 
		int m_Score;
		bool m_TurnForPlayerX;
		private App app;

		Available available;

		public List<int[]> Moves {
			get { return moves; }
			set { moves = value; }
		}

		public int[][] Board {
			get { return board; }
		}

		public int RecursiveScore
		{
			get;
			private set;
		}
		public bool GameOver
		{
			get;
			private set;
		}

		public AI_MiniMax(int[][] values, bool turnForPlayerX) //konstruktor
		{
			app = App.Instance;
			m_TurnForPlayerX = turnForPlayerX;
			board = values;
			available = new Available();
			available.boards = new List<int[][]>();
			available.moves = new List<List<int[]>>();

			ComputeScore();
		}

		public bool IsTerminalNode() //sprawdzanie czy na planszy znajduje się przynajmniej jeden pionek z danej drużyny
		{
			if (GameOver)
				return true;
			bool p1=false, p2=false;
			for (int i=0;i<board.Length;i++)
			{
				for (int j = 0; j < board[i].Length; j++)
					if (board[i][j] == 1)
						p1 = true;
					else if (board[i][j] == 2)
						p2 = true;
			}
			if (p1 && p2)
				return false;
			else
				return true;
		}

		public IEnumerable<AI_MiniMax> GetChildren() //pobieranie plansz dostępnych przy następnym ruchu
		{
			FindAvailableBoards(board, m_TurnForPlayerX ? 1 : 2);

			for (int i = 0; i < available.boards.Count; i++)
			{
				int[][] newValues = (int[][])available.boards[i].Clone();
				AI_MiniMax retChild = new AI_MiniMax(newValues, !m_TurnForPlayerX);
				retChild.Moves = new List<int[]> (available.moves [i]);
				yield return retChild;
			}
		}

        //http://www.ocf.berkeley.edu/~yosenl/extras/alphabeta/alphabeta.html
        //metoda implementująca strategię MiniMax z AlfaBeta cięciami
        public int MiniMax(int depth, bool needMax, int alpha, int beta, out AI_MiniMax childWithMax) 
		{
			childWithMax = null;
			if (depth == 0 || IsTerminalNode()) //zwrocenie wyniku przy odpowiedniej glebokosci lub w przypadku konca gry
			{
				RecursiveScore = m_Score;
				return m_Score;
			}

			foreach (AI_MiniMax cur in GetChildren())
			{
				AI_MiniMax dummy;
				int score = cur.MiniMax(depth - 1, !needMax, alpha, beta, out dummy);
				if (!needMax)
				{
					if (beta > score)
					{
						beta = score;
						childWithMax = cur;
						if (alpha >= beta)
						{
							break;
						}
					}
				}
				else
				{
					if (alpha < score)
					{
						alpha = score;
						childWithMax = cur;
						if (alpha >= beta)
						{
							break;
						}
					}
				}
			}

			RecursiveScore = needMax ? alpha : beta;
			return RecursiveScore;
		}

		public AI_MiniMax FindNextMove(int depth) //metoda wywoływana zewnętrznie, uruchamia wyszkianie następnego ruchu
		{
			AI_MiniMax ret = null;
			MiniMax(depth, m_TurnForPlayerX, int.MinValue + 1, int.MaxValue - 1, out ret);
			return ret;
		}


		void ComputeScore() //przeliczanie punktów
		{
            int[] points = new int[2];
            points[0] = 9;
            points[1] = 9;
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 1)
                        points[1] -= 1;
                    else if (board[i][j] == 2)
                        points[0] -= 1;
                }
            }

            if (points[1] == 9 || points[0] == 9)
            {
                GameOver = true;
            }
            m_Score = (points[0] - points[1]);
		}


		private void FindAvailableBoards(int[][] board, int turn) //metoda buduje dostępne plansze na podstawie dostępnych ruchów
		{
			available.boards.Clear();
			bool captureAvailable = false;
			for (int rbI = 0; rbI < board.Length; rbI++)
			{
				for (int rbJ = 0; rbJ < board[rbI].Length; rbJ++)
				{
					if (board[rbI][rbJ] != turn)
						continue;
					List<int[]> tmpAvailableMoves = app.controller.logic.checking(turn, rbI, rbJ, board); //pobranie wszystkich możliwych ruchów dla danego pionka
					if (app.controller.logic.capture && !captureAvailable)
					{
						available.boards.Clear();
						available.moves.Clear ();
						captureAvailable = true;
					}
					else if (captureAvailable && !app.controller.logic.capture)
						continue;

					for (int k = 0; k < tmpAvailableMoves.Count; k++)
					{
						int[][] boardA;
						boardA = new int[3][];
						List<int[]> moveA = new List<int[]>();
						moveA.Add (new int[]{ rbI, rbJ });
						moveA.Add (tmpAvailableMoves [k]);
						for (int l = 0; l < 3; l++)
						{
							boardA[l] = new int[7];
							for (int m = 0; m < 7; m++)
							{
								if (l == tmpAvailableMoves[k][0] && m == tmpAvailableMoves[k][1])
								{
									boardA[l][m] = board[rbI][rbJ];
								}
								else if (l == rbI && m == rbJ)
								{
									boardA[l][m] = 0;
								}
								else
								{
									if (board[l][m] == 1)
									{
										boardA[l][m] = board[l][m];
									}
									else if(board[l][m] == 2)
									{
										boardA[l][m] = board[l][m];
									}
									else if (board[l][m] == 0)
									{
										boardA[l][m] = 0;
									}
									else
									{
										boardA[l][m] = -1;
									}
								}
							}
						}

						if (app.controller.logic.capture)
						{
							int captured_x;
							if (rbI == 1)
								captured_x = tmpAvailableMoves[k][0];
							else if (tmpAvailableMoves[k][0] == 1)
								captured_x = rbI;
							else
								captured_x = rbI + (tmpAvailableMoves[k][0] - rbI) / 2;
							int captured_y = rbJ + (tmpAvailableMoves[k][1] - rbJ) / 2;
							boardA[captured_x][captured_y] = 0;

							List<int[]> comboMoves = app.controller.logic.checking(turn, tmpAvailableMoves[k][0], tmpAvailableMoves[k][1], boardA);

							if (app.controller.logic.capture)
							{
								for (int a = 0; a < comboMoves.Count; a++)
								{
									List<int[]> moveB = new List<int[]>(moveA);
									moveB.Add (comboMoves [a]);
									available.moves.Add(moveB);
									CaptureCombo(boardA, comboMoves[a], tmpAvailableMoves[k], turn);
									app.controller.logic.capture = true;
								}
							}
							else
							{
								available.boards.Add(boardA);
								available.moves.Add(moveA);
							}
							available.boards.Add(boardA);
							available.moves.Add(moveA);
						}
						else
						{
							available.boards.Add(boardA);
							available.moves.Add (moveA);
						}
					}

					app.controller.logic.capture = false;

				}
			}
		}

		private void CaptureCombo(int[][] boardA, int[] x_y_after, int[] x_y_before, int turn) //metoda wspomagająca budowanie dostępnych plansz w przypadku wielokrotnych bić
		{
			int[][] boardB;
			boardB = new int[3][];
			for (int l = 0; l < 3; l++)
			{
				boardB[l] = new int[7];
				for (int m = 0; m < 7; m++)
				{
					if (x_y_after[0] == l && x_y_after[1] == m)
					{
						boardB[l][m] = turn;
					}
					else
					{
						boardB[l][m] = boardA[l][m];
					}
				}
			}
			int captured_x;
			if (x_y_before[0] == 1)
				captured_x = x_y_after[0];
			else if (x_y_after[0] == 1)
				captured_x = x_y_before[0];
			else
				captured_x = x_y_before[0] + (x_y_after[0] - x_y_before[0]) / 2;
			int captured_y = x_y_before[1] + (x_y_after[1] - x_y_before[1]) / 2;
			boardB[captured_x][captured_y] = 0;

			boardB[x_y_before[0]][x_y_before[1]] = 0;

			List<int[]> comboMoves = app.controller.logic.checking(turn, x_y_after[0], x_y_after[1], boardB);
			if (app.controller.logic.capture)
			{
				for (int a = 0; a < comboMoves.Count; a++)
				{

					available.moves[available.moves.Count-1].Add(comboMoves[a]);
					CaptureCombo(boardB, comboMoves[a], x_y_after, turn);
					app.controller.logic.capture = true;
				}
			}
			else
			{
				available.boards.Add(boardB);
			}
		}
	}
}
