﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Advertisements;
using System;

public class MenuController : MonoBehaviour {

	// Timer and ads
	private const int TIME_20MIN = 1200;
	private const float HALF_ALPHA = (125f/255f);
	private bool shouldSetTimer = false;

	private DateTime lastWatchedAd;
	public Text timer;
	public Button watchAd;

	// To controll animation in Menu
	private const string MAIN_TO_SHOP_TRIGGER = "MainToShop";
	private const string MAIN_TO_OPTIONS_TRIGGER = "MainToOptions";
	private const string SHOP_TO_MAIN_TRIGGER = "ShopToMain";
	private const string OPTIONS_TO_MAIN_TRIGGER = "OptionsToMain";

	public Animator animator;

	// All Buttons in scene Menu
	public Button startGameButton;
	public Button shopButton;
	public Button exitShopButton;
	public Button optionsButton;
	public Button exitOptions;

	void Start () {
		startGameButton.onClick.AddListener (StartGame);
		shopButton.onClick.AddListener (GoToShop);
		exitShopButton.onClick.AddListener (ExitShop);
		optionsButton.onClick.AddListener (GoToOptions);
		exitOptions.onClick.AddListener (ExitOptions);
	}

	public void StartGame(){
		print ("TODO: StartGame");
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

	//TODO Ads

	public void ShowAd()
	{
		//Just for now before ads
		MPlugin.Instance.SaveData.LastWatchedAd = DateTime.Now;
		MPlugin.Client.Save (MPlugin.Instance.SaveData);
		SetTimes ();
		// Up to remove



//		if (Advertisement.IsReady())
//		{
//			Advertisement.Show("video", new ShowOptions() {resultCallback = HandleAdResult});
//		}
	}

//	private void HandleAdResult(ShowResult result)
//	{
//		switch (result)
//		{
//		case ShowResult.Finished:
//			Debug.Log("USER FINISHED TO WATCH AD");
//			MPlugin.Instance.SaveData.Coins += 50;
//			MPlugin.Instance.SaveData.LastWatchedAd = DateTime.Now;
//			MPlugin.Client.Save(MPlugin.Instance.SaveData);
//			SetTimes();
//			break;
//		case ShowResult.Skipped:
//			Debug.Log("USER SKIPPED AD");
//			break;
//		case ShowResult.Failed:
//			Debug.Log("FAILED TO SHOW AD");
//			break;
//		}
//	}

}
