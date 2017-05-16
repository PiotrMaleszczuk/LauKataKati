using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class GameOverController : MonoBehaviour {

	private const string SCENE_GAMEPLAY = "Gameplay";
	private const string SCENE_MENU = "Menu";

	private readonly Color GREEN = new Color (0f, 1f, 0f, 1f);
	private readonly Color RED = new Color (1f, 0f, 0f, 1f);

	public Button homeButton;
	public Button restartButton;
	public Text infoText;

	private App app;
	private Transform gameOverUI;

	public void Init () {
		app = App.Instance;
		gameOverUI = app.view.gameOverUI;

		homeButton.onClick.AddListener (Home);
		restartButton.onClick.AddListener (Restart);
	}

	public void GameOver(bool winner){
		if (winner == false) {
			app.controller.gameOver.infoText.color = RED;
			app.controller.gameOver.infoText.text = "YOU'RE the LOSER";
		} 
		else {
			app.controller.gameOver.infoText.color = GREEN;
			app.controller.gameOver.infoText.text = "YOU'RE the WINNER";
		}
		gameOverUI.gameObject.SetActive (true);
		ShowAd ();
	}

	private void Restart(){
		SceneManager.LoadScene (SCENE_GAMEPLAY);
	}

	private void Home(){
		SceneManager.LoadScene (SCENE_MENU);
	}

	public void ShowAd()
	{
		#if UNITY_ANDROID
		Advertisement.Initialize("1374173", false);
		#elif UNITY_IOS
		Advertisement.Initialize("1374174", false);
		#endif

		if (Advertisement.IsReady())
		{
			Advertisement.Show("video");
		}
	}
}
