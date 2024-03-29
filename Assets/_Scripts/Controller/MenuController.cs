﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using System;

public class MenuController : MonoBehaviour {

	private enum State {
		Selected,
		Unselected,
		ToBuy
	}
	private State state;

    //Exit game
    private bool onceClickedExit = false;

	// Timer and ads
	private const int TIME_20MIN = 1200;
	private const float HALF_ALPHA = (125f/255f);
	private bool shouldSetTimer = false;
	private DateTime lastWatchedAd;

	[Header("Timer and ads")]
	public Text timer;
	public Button watchAd;

	// Shop
	[Header("Shop")]
	public GameObject pawn1;
	public GameObject pawn2;

	public Text coinsText;
	public Text priceText;
	public Button leftArrow;
	public Button rightArrow;
	public Button buyButton;
	public Animator shopAnimator;

	private Text leftArrowText;
	private Text rightArrowText;
	private Text buyButtonText;

	[Header("Shop items")]
	public Animator priceAnimator;
	public Image pawn1Image;
	public Image pawn2Image;
	public int[] prices;
	public Sprite[] pawn1Sprites;
	public Sprite[] pawn2Sprites;

	private int currentPosition = 0;
	private bool isMoveAnim = false;

	// Options
	[Header("Options items")]
	public Button soundButton;
	public Text soundButtonText;
	public Button forceBeatingButton;
	public Text forceBeatingText;
	public Button creditsButton;
	public Button creditsBack;

	// To controll animation in Menu
	[Header("To controll animation in Menu")]
	public Animator animator;

	private const string MAIN_TO_SHOP_TRIGGER = "MainToShop";
	private const string MAIN_TO_OPTIONS_TRIGGER = "MainToOptions";
	private const string SHOP_TO_MAIN_TRIGGER = "ShopToMain";
	private const string OPTIONS_TO_MAIN_TRIGGER = "OptionsToMain";
	private const string MAIN_TO_MODE_TRIGGER = "MainToModeChooser";
	private const string MODE_TO_MAIN_TRIGGER = "ModeChooserToMain";
	private const string NOT_ENOUGHT_MONEY_TRIGGER = "NotEnoughtMoney";
	private const string OPTIONS_TO_CREDITS_TRIGGER = "OptionToCredits";
	private const string CREDITS_TO_OPTIONS = "CreditsToOptions";
	private const string MOVE_SHOP_SKIN_BOOL = "Move";
	private const string SCENE_GAMEPLAY = "Gameplay";

	// All Buttons in scene Menu
	[Header("All Buttons in scene Menu")]
	public Button startGameButton;
	public Button shopButton;
	public Button exitShopButton;
	public Button optionsButton;
	public Button exitOptions;
	public Button singlePlayerGame;
	public Button localMultiplayerGame;
	public Button multiplayerGame;
	public Button backBackgroundButton;
    public Button multiplayerServerButton;
    public Button multiplayerClientButton;
    public Button multiplayerServerClientExit;
    public GameObject multiplayerServerClientWindow;

    void Awake() {
		#if UNITY_ANDROID
		Advertisement.Initialize("1374173", false);
		#elif UNITY_IOS
		Advertisement.Initialize("1374174", false);
		#endif
		Application.targetFrameRate = 60;
	}

	void Start () {
		singlePlayerGame.onClick.AddListener (SinglePlayerGame);
		localMultiplayerGame.onClick.AddListener (LocalMultiPlayerGame);
		multiplayerGame.onClick.AddListener (MultiplayerGame);
		backBackgroundButton.onClick.AddListener (BackBackgroundAction);
		startGameButton.onClick.AddListener (StartGame);
		shopButton.onClick.AddListener (GoToShop);
		exitShopButton.onClick.AddListener (ExitShop);
		optionsButton.onClick.AddListener (GoToOptions);
		exitOptions.onClick.AddListener (ExitOptions);
		leftArrow.onClick.AddListener (ShopLeftArrow);
		rightArrow.onClick.AddListener (ShopRightArrow);
		buyButton.onClick.AddListener (BuyButtonAction);
		soundButton.onClick.AddListener (SoundsButton);
		forceBeatingButton.onClick.AddListener (ForceBeatingButton);
		creditsButton.onClick.AddListener (CreditsButton);
		creditsBack.onClick.AddListener (CreditsBackButton);
        multiplayerServerButton.onClick.AddListener(MultiplayerServerButtonAction);
        multiplayerClientButton.onClick.AddListener(MultiplayerClientButtonAction);
        multiplayerServerClientExit.onClick.AddListener(MultiplayerWindowExitButtonAction);

        leftArrowText = leftArrow.GetComponentInChildren<Text> ();
		rightArrowText = rightArrow.GetComponentInChildren<Text> ();
		priceAnimator = priceText.GetComponent<Animator> ();
		buyButtonText = buyButton.GetComponentInChildren<Text> ();

		currentPosition = SaveDataController.Instance.Data.selectedSkin;
		SetCoinsStatus ();
		SetPrice (prices [currentPosition]);
		SetArrowsView ();
		SetBuyButtonText ();
		SetPawnView ();
		SetPawnPrefabs ();
		SetSoundsAndForceBeatingTexts ();
		SetTimes();
	}

	public void SinglePlayerGame(){
        SaveDataController.Instance.Data.mode = 1;
        SaveDataController.Instance.Data.Save();
        SceneManager.LoadScene (SCENE_GAMEPLAY);
	}

	public void LocalMultiPlayerGame(){
        SaveDataController.Instance.Data.mode = 2;
        SaveDataController.Instance.Data.Save();
        SceneManager.LoadScene(SCENE_GAMEPLAY);
    }

    public void MultiplayerGame()
    {
        this.multiplayerServerClientWindow.SetActive(true);
    }

    public void MultiplayerServerButtonAction()
    {
        SaveDataController.Instance.Data.mode = 3;
        SaveDataController.Instance.Data.Save();
        SceneManager.LoadScene(SCENE_GAMEPLAY);
    }

    public void MultiplayerClientButtonAction()
    {
        SaveDataController.Instance.Data.mode = 4;
        SaveDataController.Instance.Data.Save();
        SceneManager.LoadScene(SCENE_GAMEPLAY);
    }

    public void MultiplayerWindowExitButtonAction()
    {
        this.multiplayerServerClientWindow.SetActive(false);
    }

	public void StartGame(){
		animator.ResetTrigger (MODE_TO_MAIN_TRIGGER);
		animator.SetTrigger (MAIN_TO_MODE_TRIGGER);
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

	public void BackBackgroundAction(){
		animator.SetTrigger (MODE_TO_MAIN_TRIGGER);
	}

	public void ShopRightArrow(){
		if (isMoveAnim) return;
		if (currentPosition == (prices.Length - 1)) {
			return;
		}

		currentPosition++;
		SetPrice(prices[currentPosition]);
		SetArrowsView ();
		StartCoroutine (MovePawnShopAnimation());
	}

	public void ShopLeftArrow(){
		if (isMoveAnim) return;
		if (currentPosition == 0) {
			return;
		}

		currentPosition--;
		SetPrice(prices[currentPosition]);
		SetArrowsView ();
		StartCoroutine (MovePawnShopAnimation());
	}

	public void BuyButtonAction(){
		if (state == State.Selected) {
			return;
		} else if (state == State.Unselected) {
			SaveDataController.Instance.Data.selectedSkin = currentPosition;
			SaveDataController.Instance.Data.Save ();
			SetPawnPrefabs ();
			SetBuyButtonText ();
		} else if (state == State.ToBuy) {
			if (SaveDataController.Instance.Data.coins >= prices [currentPosition]) {
				bool[] owned = SaveDataController.Instance.Data.skinOwned;
				owned [currentPosition] = true;
				SaveDataController.Instance.Data.skinOwned = owned;
				SaveDataController.Instance.Data.coins -= prices [currentPosition];
				SaveDataController.Instance.Data.Save ();
				SetBuyButtonText ();
				SetCoinsStatus ();
			} else {
				priceAnimator.SetTrigger (NOT_ENOUGHT_MONEY_TRIGGER);
			}
		}
	}

	public void SoundsButton(){
		bool sounds = SaveDataController.Instance.Data.sounds;
		if (sounds) {
			SaveDataController.Instance.Data.sounds = false;
		} else {
			SaveDataController.Instance.Data.sounds = true;
		}

		SetSoundsAndForceBeatingTexts ();
	}

	public void ForceBeatingButton(){
		bool forceBeating = SaveDataController.Instance.Data.isForceBeating;
		if (forceBeating) {
			SaveDataController.Instance.Data.isForceBeating = false;
		} else {
			SaveDataController.Instance.Data.isForceBeating = true;
		}

		int difficult = SaveDataController.Instance.Data.difficult;
		if (difficult == 9) {
			SaveDataController.Instance.Data.difficult = 1;
		} else {
			SaveDataController.Instance.Data.difficult += 1;
		}

		SetSoundsAndForceBeatingTexts ();
	}

	public void CreditsButton(){
		animator.SetTrigger (OPTIONS_TO_CREDITS_TRIGGER);
	}

	public void CreditsBackButton(){
		animator.SetTrigger (CREDITS_TO_OPTIONS);
	}

	void Update()
	{
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("main"))
        {
            print("main");
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                print("escape");
                if (onceClickedExit)
                    Application.Quit();
                else
                    onceClickedExit = true;
            }
        }
        else if (onceClickedExit)
        {
            print("nie main");
            onceClickedExit = false;
        }
            
        if (!shouldSetTimer) return;
		SetTimer();
	}

	private void SetTimes()
	{
		var currentTime = new DateTime(DateTime.Now.Ticks);
		lastWatchedAd = new DateTime(SaveDataController.Instance.Data.LastWatchedAd.Ticks);

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

		if (time.Seconds < 0) {
			SetTimes ();
		}
	}

	public void SetCoinsStatus(){
		coinsText.text = "You have: " + SaveDataController.Instance.Data.coins + " COINS";
	}

	private void SetPrice(int price){
		priceText.text = "Price: " + price;
	}

	private void SetArrowsView(){
		if (currentPosition == 0) {
			leftArrowText.color = new Color (leftArrowText.color.r, leftArrowText.color.g, leftArrowText.color.b, 0.1f);
			rightArrowText.color = new Color (rightArrowText.color.r, rightArrowText.color.g, rightArrowText.color.b, 1f);
		} else if (currentPosition == (prices.Length - 1)) {
			leftArrowText.color = new Color (leftArrowText.color.r, leftArrowText.color.g, leftArrowText.color.b, 1f);
			rightArrowText.color = new Color (rightArrowText.color.r, rightArrowText.color.g, rightArrowText.color.b, 0.1f);
		} else {
			leftArrowText.color = new Color (leftArrowText.color.r, leftArrowText.color.g, leftArrowText.color.b, 1f);
			rightArrowText.color = new Color (rightArrowText.color.r, rightArrowText.color.g, rightArrowText.color.b, 1f);
		}
	}

	private void SetPawnView(){
		pawn1Image.sprite = pawn1Sprites [currentPosition];
		pawn2Image.sprite = pawn2Sprites [currentPosition];
	}

	private void SetPawnPrefabs(){
		pawn1.GetComponent<SpriteRenderer> ().sprite = pawn1Sprites [currentPosition];
		pawn2.GetComponent<SpriteRenderer> ().sprite = pawn2Sprites [currentPosition];
	}

	private void SetBuyButtonText(){
		bool[] owned = SaveDataController.Instance.Data.skinOwned;
		int selected = SaveDataController.Instance.Data.selectedSkin;

		if (owned [currentPosition]) {
			if (selected == currentPosition) {
				buyButtonText.text = "SELECTED";
				state = State.Selected;
			} else {
				buyButtonText.text = "TAP TO CHOOSE";
				state = State.Unselected;
			}
		} else {
			buyButtonText.text = "BUY";
			state = State.ToBuy;
		}
	}

	private void SetSoundsAndForceBeatingTexts(){
		bool sounds = SaveDataController.Instance.Data.sounds;
		if (sounds) {
			soundButtonText.text = "ON";
		} else {
			soundButtonText.text = "OFF"; 
		}

		int difficult = SaveDataController.Instance.Data.difficult;
		forceBeatingText.text = difficult.ToString();
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
		string resultString  ="";
		
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("USER FINISHED TO WATCH AD");
			SaveDataController.Instance.Data.coins += 10;
			SaveDataController.Instance.Data.LastWatchedAd = DateTime.Now;
			SaveDataController.Instance.Data.Save ();
			SetCoinsStatus ();
			SetTimes ();
			resultString = "Finished";
			break;
		case ShowResult.Skipped:
			Debug.Log ("USER SKIPPED AD");
			resultString = "Skipped";
			break;
		case ShowResult.Failed:
			Debug.Log ("FAILED TO SHOW AD");
			resultString = "Failed";
			break;
		}
		DataCollectingController.Instance.SaveData("Advertisement - Place: shop | Result: "+resultString+" | Time: "+System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy"));
	}

	private IEnumerator MovePawnShopAnimation() {
		isMoveAnim = true;
		bool run = true;
		bool run2 = true;

		shopAnimator.SetBool (MOVE_SHOP_SKIN_BOOL, true);
		yield return new WaitForSecondsRealtime(0.1f);

		while(run) {
			if(!shopAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShopPawnRightAnimation")) {
				SetPawnView();
				SetBuyButtonText ();
				shopAnimator.SetBool (MOVE_SHOP_SKIN_BOOL, false);
				run = false;
			}
			yield return null;
		}

		yield return new WaitForSecondsRealtime(0.1f);

		while(run2) {
			if(!shopAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShopPawnLeftAnimation")) {
				isMoveAnim = false;
				run2 = false;
			}
			yield return null;
		}
	}
}
