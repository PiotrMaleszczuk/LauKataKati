﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class App : MonoBehaviour {
	
    public static App Instance;
    
    [Serializable]
    public class View {
        public Transform points;
        public Transform board;
        public Transform empty;
        public Transform glow;
    }

    [Serializable]
    public class Model {
        public GameObject pawn1;
        public GameObject pawn2;
        public GameObject empty;
        public GameObject glows;
        public Text turnText;
    }

    [Serializable]
    public class Controller {
		public GameController game;
        public BoardController board;
        public ClickController click;
		public LogicController logic;
        public GlowController glow;
    }

    public Model model;
    public View view;
    public Controller controller;


    void Awake() {
        App.Instance = this;
    }

}
