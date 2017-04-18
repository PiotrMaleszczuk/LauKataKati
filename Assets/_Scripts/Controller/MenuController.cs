using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using System;

public class MenuController : MonoBehaviour {

	// Timer and ads
	private const int TIME_20MIN = 1200;
	private const float HALF_ALPHA = (125f/255f);
	private bool shouldSetTimer = false;

	private DateTime lastWatchedAd;
	public Text timer;
	public Button watchAd;

	// Shop
	public Text coinsText;
	public Button leftArrow;
	public Button rightArrow;

	public Animator shopAnimator;

	private const string RIGHT_ARROW_TRIGGER = "Right";
	private const string LEFT_ARROW_TRIGGER = "Left";

	// To controll animation in Menu
	private const string MAIN_TO_SHOP_TRIGGER = "MainToShop";
	private const string MAIN_TO_OPTIONS_TRIGGER = "MainToOptions";
	private const string SHOP_TO_MAIN_TRIGGER = "ShopToMain";
	private const string OPTIONS_TO_MAIN_TRIGGER = "OptionsToMain";

	private const string SCENE_GAMEPLAY = "Gameplay";

	public Animator animator;

	// All Buttons in scene Menu
	public Button startGameButton;
	public Button shopButton;
	public Button exitShopButton;
	public Button optionsButton;
	public Button exitOptions;

	void Awake() {
		#if UNITY_ANDROID
		Advertisement.Initialize("1374173", false);
		#elif UNITY_IOS
		Advertisement.Initialize("1374174", false);
		#endif
	}

	void Start () {
		startGameButton.onClick.AddListener (StartGame);
		shopButton.onClick.AddListener (GoToShop);
		exitShopButton.onClick.AddListener (ExitShop);
		optionsButton.onClick.AddListener (GoToOptions);
		exitOptions.onClick.AddListener (ExitOptions);
		leftArrow.onClick.AddListener (ShopLeftArrow);
		rightArrow.onClick.AddListener (ShopRightArrow);

		SetCoinsStatus ();
	}

	public void StartGame(){
		SceneManager.LoadScene (SCENE_GAMEPLAY);
	}

	public void GoToShop(){
		animator.SetTrigger (MAIN_TO_SHOP_TRIGGER);
	}

	public void GoToOptions(){
		animator.SetTrigger (MAIN_TO_OPTIONS_TRIGGER);
	}

	public void ExitShop(){
		animator.SetTrigger (SHOP_TO_MAIN_TRIGGER);
	}

	public void ExitOptions(){
		animator.SetTrigger (OPTIONS_TO_MAIN_TRIGGER);
	}

	public void ShopRightArrow(){
		shopAnimator.SetTrigger (RIGHT_ARROW_TRIGGER);
	}

	public void ShopLeftArrow(){
		shopAnimator.SetTrigger (LEFT_ARROW_TRIGGER);
	}

	void Update()
	{
		if (!shouldSetTimer) return;
		SetTimer();
	}

	void OnEnable()
	{
		SetTimes();
	}

	private void SetTimes()
	{
		var currentTime = new DateTime(DateTime.Now.Ticks);
		lastWatchedAd = new DateTime(MPlugin.Instance.SaveData.LastWatchedAd.Ticks);

		var diff = currentTime.Subtract(lastWatchedAd);
		if (diff.TotalSeconds > TIME_20MIN)
		{
			timer.text = "Now !";
			watchAd.onClick.AddListener(ShowAd);
			var color = watchAd.image.color;
			watchAd.image.color = new Color(color.r, color.g, color.b, 1f);
			shouldSetTimer = false;
		}
		else
		{
			watchAd.onClick.RemoveAllListeners();
			var color = watchAd.image.color;
			watchAd.image.color = new Color(color.r, color.g, color.b, HALF_ALPHA);
			shouldSetTimer = true;
		}
	}

	private void SetTimer()
	{
		var currentTime = new DateTime(DateTime.Now.Ticks);
		var diff = currentTime.Subtract(lastWatchedAd);

		var time = TimeSpan.FromSeconds(TIME_20MIN - diff.TotalSeconds);
		timer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
	}

	public void SetCoinsStatus(){
		coinsText.text = "You have: " + MPlugin.Instance.SaveData.coins + " COINS";
	}
		
	public void ShowAd()
	{
		if (Advertisement.IsReady())
		{
			Advertisement.Show("video", new ShowOptions() {resultCallback = HandleAdResult});
		}
	}

	private void HandleAdResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("USER FINISHED TO WATCH AD");
			MPlugin.Instance.SaveData.coins += 10;
			MPlugin.Instance.SaveData.LastWatchedAd = DateTime.Now;
			MPlugin.Client.Save (MPlugin.Instance.SaveData);
			SetCoinsStatus ();
			SetTimes();
			break;
		case ShowResult.Skipped:
			Debug.Log("USER SKIPPED AD");
			break;
		case ShowResult.Failed:
			Debug.Log("FAILED TO SHOW AD");
			break;
		}
	}

}
