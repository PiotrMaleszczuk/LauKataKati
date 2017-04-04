using System;
using UnityEngine;
using System.Collections;

public class App : MonoBehaviour {
	
    public static App Instance;
    
    [Serializable]
    public class View {
		public Transform board;
    }

    [Serializable]
    public class Model {
        
    }

    [Serializable]
    public class Controller {
		public GameController game;
        public BoardController board;
    }

    public Model model;
    public View view;
    public Controller controller;


    void Awake() {
        App.Instance = this;
    }

}
