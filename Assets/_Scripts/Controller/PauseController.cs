using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {

	private const string SCENE_GAMEPLAY = "Gameplay";
	private const string SCENE_MENU = "Menu";
	private App app;
	private Transform pauseUI;

[Header("Pause Elements")]
	public Button pauseButton;
	public Button continueButton;
	public Button homeButton;
	public Button restartButton;

	public void Init(){
		app = App.Instance;
		pauseUI = app.view.pauseUI;

		pauseButton.onClick.AddListener (Pause);
		continueButton.onClick.AddListener (Continue);
		homeButton.onClick.AddListener (Home);
		restartButton.onClick.AddListener (Restart);
	}

	private void Pause(){
		pauseUI.gameObject.SetActive (true);
	}

	private void Continue(){
		pauseUI.gameObject.SetActive (false);
	}

	private void Home(){
		app.controller.bluetooth.StopConnection ();
		SceneManager.LoadScene (SCENE_MENU);
	}

	private void Restart(){
		SceneManager.LoadScene (SCENE_GAMEPLAY);
	}
}
