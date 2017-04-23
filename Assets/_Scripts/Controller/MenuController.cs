using System.Collections;
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

	// To controll animation in Menu
	[Header("To controll animation in Menu")]
	public Animator animator;

	private const string MAIN_TO_SHOP_TRIGGER = "MainToShop";
	private const string MAIN_TO_OPTIONS_TRIGGER = "MainToOptions";
	private const string SHOP_TO_MAIN_TRIGGER = "ShopToMain";
	private const string OPTIONS_TO_MAIN_TRIGGER = "OptionsToMain";
	private const string NOT_ENOUGHT_MONEY_TRIGGER = "NotEnoughtMoney";
	private const string MOVE_SHOP_SKIN_BOOL = "Move";
	private const string SCENE_GAMEPLAY = "Gameplay";

	// All Buttons in scene Menu
	[Header("All Buttons in scene Menu")]
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
		buyButton.onClick.AddListener (BuyButtonAction);
		soundButton.onClick.AddListener (SoundsButton);
		forceBeatingButton.onClick.AddListener (ForceBeatingButton);
		creditsButton.onClick.AddListener (CreditsButton);

		leftArrowText = leftArrow.GetComponentInChildren<Text> ();
		rightArrowText = rightArrow.GetComponentInChildren<Text> ();
		priceAnimator = priceText.GetComponent<Animator> ();
		buyButtonText = buyButton.GetComponentInChildren<Text> ();

		currentPosition = SaveDataController.Instance.Data.selectedSkin;
		SetCoinsStatus ();
		SetPrice (prices [currentPosition]);
		SetArrowsView ();
		SetBuyButtonText ();
		SetSoundsAndForceBeatingTexts ();
		SetTimes();
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

		SetSoundsAndForceBeatingTexts ();
	}

	public void CreditsButton(){
		print ("TODO Credist here");
	}

	void Update()
	{
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

		bool forceBeating = SaveDataController.Instance.Data.isForceBeating;
		if (forceBeating) {
			forceBeatingText.text = "ON";
		} else {
			forceBeatingText.text = "OFF";
		}
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
			SaveDataController.Instance.Data.coins += 10;
			SaveDataController.Instance.Data.LastWatchedAd = DateTime.Now;
			SaveDataController.Instance.Data.Save ();
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
