using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	private const string MAIN_TO_SHOP_TRIGGER = "MainToShop";
	private const string MAIN_TO_OPTIONS_TRIGGER = "MainToOptions";
	private const string SHOP_TO_MAIN_TRIGGER = "ShopToMain";
	private const string OPTIONS_TO_MAIN_TRIGGER = "OptionsToMain";

	public Animator animator;

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

}
