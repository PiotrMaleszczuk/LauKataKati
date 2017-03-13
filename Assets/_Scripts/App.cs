using System;
using UnityEngine;
using System.Collections;

public class App : MonoBehaviour {
	
    public static App Instance;
    
    [Serializable]
    public class View {

    }

    [Serializable]
    public class Model {
        
    }

    [Serializable]
    public class Controller {
        
    }

    public Model model;
    public View view;
    public Controller controller;


    void Awake() {
        App.Instance = this;
    }

}
