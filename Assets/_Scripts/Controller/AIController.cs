using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AIController : MonoBehaviour {
    private App app;



    public void Init()
    {
        app = App.Instance;

    }

    public int[][] ComputerMakeMove(int depth)
    {
        bool t;
        if (app.controller.turns.GetTurn() == 1)
            t = true;
        else if (app.controller.turns.GetTurn() == 2)
            t = false;
        else
            return null;
        Board Current = new Board(app.controller.board.Board, t);
        Board next = Current.FindNextMove(depth);
        if (next != null)
            return next.m_Values;
        return null;
    }

    sealed class Board
    {
        public int[][] m_Values;
        int m_Score;
        bool m_TurnForPlayerX;
        private App app;
        List<int[][]> availableBoards;


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

        public Board(int[][] values, bool turnForPlayerX)
        {
            app = App.Instance;
            m_TurnForPlayerX = turnForPlayerX;
            m_Values = values;
            availableBoards = new List<int[][]>();
            ComputeScore();
        }


        /*
        public Board GetChildAtPosition(int x, int y)
        {
            int i = x + y * 3;
            int[][] newValues = (int[][])m_Values.Clone();

            if (m_Values[i] != 0)
                print("invalid index ["+ x+"," + y+"] is taken by "+ m_Values[i]);

            newValues[i] = m_TurnForPlayerX ? GridEntry.PlayerX : GridEntry.PlayerO;
            return new Board(newValues, !m_TurnForPlayerX);
        }*/

        public bool IsTerminalNode()
        {
            if (GameOver)
                return true;
            //if all entries are set, then it is a leaf node
            bool p1=false, p2=false;
            for (int i=0;i<m_Values.Length;i++)
            {
                for (int j = 0; j < m_Values[i].Length; j++)
                    if (m_Values[i][j] == 1)
                        p1 = true;
                    else if (m_Values[i][j] == 2)
                        p2 = true;
            }
            if (p1 && p2)
                return false;
            else
                return true;
        }

        public IEnumerable<Board> GetChildren()
        {
            FindAvailableBoards(m_Values, m_TurnForPlayerX ? 1 : 2);

            for (int i = 0; i < availableBoards.Count; i++)
            {
                int[][] newValues = (int[][])availableBoards[i].Clone();
                yield return new Board(newValues, !m_TurnForPlayerX);
            }
        }

        //http://en.wikipedia.org/wiki/Alpha-beta_pruning
        public int MiniMaxShortVersion(int depth, int alpha, int beta, out Board childWithMax)
        {
            childWithMax = null;
            if (depth == 0 || IsTerminalNode())
            {
                //When it is turn for PlayO, we need to find the minimum score.
                RecursiveScore = m_Score;
                return m_TurnForPlayerX ? m_Score : -m_Score;
            }

            foreach (Board cur in GetChildren())
            {
                Board dummy;
                int score = -cur.MiniMaxShortVersion(depth - 1, -beta, -alpha, out dummy);
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

            RecursiveScore = alpha;
            return alpha;
        }

        //http://www.ocf.berkeley.edu/~yosenl/extras/alphabeta/alphabeta.html
        public int MiniMax(int depth, bool needMax, int alpha, int beta, out Board childWithMax)
        {
            childWithMax = null;
            System.Diagnostics.Debug.Assert(m_TurnForPlayerX == needMax);
            if (depth == 0 || IsTerminalNode())
            {
                RecursiveScore = m_Score;
                return m_Score;
            }

            foreach (Board cur in GetChildren())
            {
                Board dummy;
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

        public Board FindNextMove(int depth)
        {
            Board ret = null;
            Board ret1 = null;
            MiniMax(depth, m_TurnForPlayerX, int.MinValue + 1, int.MaxValue - 1, out ret);

            /*
            MiniMaxShortVersion(depth, int.MinValue + 1, int.MaxValue - 1, out ret1);

            //compare the two versions of MiniMax give the same results
            if (!IsSameBoard(ret, ret1, true))
            {
                print("ret="+ ret+"\n,!= ret1="+ ret1+",\ncur=");
                print("Two MinMax functions don't match.");
            }*/
            return ret;
        }
        
        int GetScoreForOneLine()
        {
            int countX = 0, countO = 0;
            int[] points = new int[2];
            points[0] = 9;
            points[1] = 9;
            for (int i = 0; i < m_Values.Length; i++)
            {
                for (int j = 0; j < m_Values[i].Length; j++)
                {
                    if (m_Values[i][j] == 1)
                        points[1] -= 1;
                    else if (m_Values[i][j] == 2)
                        points[0] -= 1;

                }
            }
            countX = points[0];
            countO = points[1];

            if (countO == 9 || countX == 9)
            {
                GameOver = true;
            }

            //The player who has turn should have more advantage.
            //What we should have done
            int advantage = 1;
            if (countO == 0)
            {
                if (m_TurnForPlayerX)
                    advantage = 3;
                return (int)System.Math.Pow(10, countX) * advantage;
            }
            else if (countX == 0)
            {
                if (!m_TurnForPlayerX)
                    advantage = 3;
                return -(int)System.Math.Pow(10, countO) * advantage;
            }
            return 0;
        }

        void ComputeScore()
        {
            int ret = 0;

            
            ret += GetScoreForOneLine();
            m_Score = ret;
        }
        private void FindAvailableBoards(int[][] realBoard, int turn)
        {
            availableBoards.Clear();
            bool captureAvailable = false;
            for (int rbI = 0; rbI < realBoard.Length; rbI++)
            {
                for (int rbJ = 0; rbJ < realBoard[rbI].Length; rbJ++)
                {
                    if (realBoard[rbI][rbJ] != turn)
                        continue;
                    List<int[]> tmpAvailableMoves = app.controller.logic.checking(turn, rbI, rbJ, realBoard);
                    if (app.controller.logic.capture && !captureAvailable)
                    {
                        availableBoards.Clear();
                        captureAvailable = true;
                    }
                    else if (captureAvailable && !app.controller.logic.capture)
                        continue;

                    for (int k = 0; k < tmpAvailableMoves.Count; k++)
                    {
                        //print(rbI + " : " + rbJ + " | " + tmpAvailableMoves[k][0] + " : " + tmpAvailableMoves[k][1]);
                        int[][] boardA;
                        boardA = new int[3][];
                        for (int l = 0; l < 3; l++)
                        {
                            boardA[l] = new int[7];
                            for (int m = 0; m < 7; m++)
                            {
                                if (l == tmpAvailableMoves[k][0] && m == tmpAvailableMoves[k][1])
                                {
                                    boardA[l][m] = realBoard[rbI][rbJ];
                                }
                                else if (l == rbI && m == rbJ)
                                {
                                    boardA[l][m] = 0;
                                }
                                else
                                {
                                    if (realBoard[l][m] == 1)
                                    {
                                        boardA[l][m] = realBoard[l][m];
                                    }
                                    else if(realBoard[l][m] == 2)
                                    {
                                        boardA[l][m] = realBoard[l][m];
                                    }
                                    else if (realBoard[l][m] == 0)
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
                                    CaptureCombo(boardA, comboMoves[a], tmpAvailableMoves[k], turn);
                                    app.controller.logic.capture = true;
                                }
                            }
                            else
                            {
                                availableBoards.Add(boardA);
                            }
                        }
                        else
                        {
                            availableBoards.Add(boardA);
                        }
                    }

                    app.controller.logic.capture = false;
                }
            }/*
            print("FindAvailableBoards:");
            for (int i = 0; i < availableBoards.Count; i++)
                PrintBoard(availableBoards[i]);*/
        }
        private void CaptureCombo(int[][] boardA, int[] x_y_after, int[] x_y_before, int turn)
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
                    CaptureCombo(boardB, comboMoves[a], x_y_after, turn);
                    app.controller.logic.capture = true;
                }
            }
            else
            {
                availableBoards.Add(boardB);
            }

        }

        private void PrintBoard(int[][] tab)
        {
            string boardString = "";
            for (int i = 0; i < tab.Length; i++)
            {
                for (int j = 0; j < tab[i].Length; j++)
                {
                    boardString += tab[i][j];
                    boardString += " ";
                }
                boardString += "\n";
            }
            print(boardString);
        }


        static bool IsSameBoard(Board a, Board b, bool compareRecursiveScore)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            for (int i = 0; i < a.m_Values.Length; i++)
            {
                if (a.m_Values[i] != b.m_Values[i])
                    return false;
            }

            if (a.m_Score != b.m_Score)
                return false;

            if (compareRecursiveScore && Mathf.Abs(a.RecursiveScore) != Mathf.Abs(b.RecursiveScore))
                return false;

            return true;
        }
    }


}
