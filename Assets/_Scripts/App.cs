using System;
using UnityEngine;
using System.Collections;

public class App : MonoBehaviour {
	
    public static App Instance;
    
    [Serializable]
    public class View {
        public Transform points;
        public Transform board;
    }

    [Serializable]
    public class Model {
        public GameObject pawn1;
        public GameObject pawn2;
    }

    [Serializable]
    public class Controller {
		public GameController game;
        public BoardController board;
        public ClickController click;
    }

    public Model model;
    public View view;
    public Controller controller;


    void Awake() {
        App.Instance = this;
    }

}
