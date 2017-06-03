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
	private readonly Color YELLOW = new Color (1f, 0.92f, 0.016f, 1f);

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

	public void GameOver(int winner){
		switch(winner){
	        case 1:
		        app.controller.gameOver.infoText.color = GREEN;
		        app.controller.gameOver.infoText.text = "YOU'RE the WINNER";
		        SaveDataController.Instance.Data.coins += 10;
		        break;
	        case 2:
		        app.controller.gameOver.infoText.color = RED;
		        app.controller.gameOver.infoText.text = "YOU'RE the LOSER";
                SaveDataController.Instance.Data.coins += 2;
		        break;
	        case 3:
		        app.controller.gameOver.infoText.color = YELLOW;
		        app.controller.gameOver.infoText.text = "DRAW";
		        SaveDataController.Instance.Data.coins += 5;
		        break;
            case 4:
                app.controller.gameOver.infoText.color = GREEN;
		        app.controller.gameOver.infoText.text = "WHITE WIN";
                break;
            case 5:
                app.controller.gameOver.infoText.color = GREEN;
		        app.controller.gameOver.infoText.text = "BLACK WIN";
                break;
		}
		gameOverUI.gameObject.SetActive (true);
		ShowAd ();
	}

	private void Restart(){
		SceneManager.LoadScene (SCENE_GAMEPLAY);
	}

	private void Home(){
		app.controller.bluetooth.StopConnection ();
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
