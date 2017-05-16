using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class App : MonoBehaviour {
	
    public static App Instance;
    
    [Serializable]
    public class View {
        public Transform points;
        public Transform board;
        public Transform empty;
        public Transform glow;
		public Transform pauseUI;
		public Text turnText;
		public Transform gameOverUI;
    }

    [Serializable]
    public class Model {
        public GameObject pawn1;
        public GameObject pawn2;
        public GameObject empty;
        public GameObject glows;
    }

    [Serializable]
    public class Controller {
		public GameController game;
        public BoardController board;
        public ClickController click;
		public LogicController logic;
        public GlowController glow;
        public TurnController turns;
		public PauseController pause;
        public AIController ai;
		public GameOverController gameOver;
    }

    public Model model;
    public View view;
    public Controller controller;


    void Awake() {
        App.Instance = this;
    }

}
