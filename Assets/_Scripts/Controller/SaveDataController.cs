using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class SaveDataController : MonoBehaviour {

	private static SaveDataController instance;
	public static SaveDataController Instance
	{
		get { return instance; }
	}

	private Data data;
	public Data Data
	{
		get { return this.data; }
		set { this.data = value; }
	}

	void Awake()
	{
		if (SaveDataController.instance == null)
		{
			SaveDataController.instance = this;
		}

		this.data = new Data ();
		this.data.InicializePrefs ();
		DontDestroyOnLoad(this.gameObject);
	}
}

public class Data
{	
	//Keys
	private const string WIN_COUNT_KEY = "winCount";
	private const string LOST_COUNT_KEY = "lostCount";
	private const string COINS_KEY = "coins";
	private const string SKIN_OWNED_KEY = "skinOwned";
	private const string LAST_WATCHED_AD_KEY = "lastWatchedAd";
	private const string FORCE_BEATING_KEY = "forceBeating";
	private const string SOUNDS_KEY = "sounds";
	private const string FMT = "yyyy-MM-dd HH:mm:ss.fff";
	//

	public bool sounds {
		get { if (PlayerPrefs.GetInt (SOUNDS_KEY) == 1)
				return true;
			else
				return false;}
		set { if (value == true)
				PlayerPrefs.SetInt (SOUNDS_KEY, 1);
			else
				PlayerPrefs.SetInt (SOUNDS_KEY, 0);}
	}

	public bool isForceBeating {
		get { if (PlayerPrefs.GetInt (FORCE_BEATING_KEY) == 1)
			return true;
		else
			return false;}
		set { if (value == true)
			PlayerPrefs.SetInt (FORCE_BEATING_KEY, 1);
		else
			PlayerPrefs.SetInt (FORCE_BEATING_KEY, 0);}
	}

	public int winCount {
		get {  return PlayerPrefs.GetInt(WIN_COUNT_KEY); }
		set { PlayerPrefs.SetInt (WIN_COUNT_KEY, value); }
	}

	public int lostCount {
		get {  return PlayerPrefs.GetInt(LOST_COUNT_KEY); }
		set { PlayerPrefs.SetInt (LOST_COUNT_KEY, value); }
	}

	public int coins {
		get {  return PlayerPrefs.GetInt(COINS_KEY); }
		set { PlayerPrefs.SetInt (COINS_KEY, value); }
	}
		
	public bool[] skinOwned {
		get {
			bool[] tab = new bool[10];
			string stringTab = PlayerPrefs.GetString (SKIN_OWNED_KEY);
			for (int i = 0; i < 10; i++) {
				if (stringTab [i] == '1') {
					tab [i] = true;
				} else {
					tab [i] = false; 
				}
			}
			return tab;
		}
		set {
			string stringTab = "";
			for (int i = 0; i < value.Length; i++) {
				if (value [i] == true) {
					stringTab += '1';
				} else {
					stringTab += '0';
				}
			}
			PlayerPrefs.SetString (SKIN_OWNED_KEY, stringTab);
		}
	}

	public DateTime LastWatchedAd {
		get {  return DateTime.ParseExact (PlayerPrefs.GetString (LAST_WATCHED_AD_KEY), FMT, CultureInfo.InvariantCulture); }
		set {  PlayerPrefs.SetString (LAST_WATCHED_AD_KEY, value.ToString(FMT)); }
	}
		
	public void InicializePrefs(){
		if (!PlayerPrefs.HasKey(WIN_COUNT_KEY)) { PlayerPrefs.SetInt(WIN_COUNT_KEY, 0); }
		if (!PlayerPrefs.HasKey(LOST_COUNT_KEY)) { PlayerPrefs.SetInt(LOST_COUNT_KEY, 0); }
		if (!PlayerPrefs.HasKey(SOUNDS_KEY)) { PlayerPrefs.SetInt(SOUNDS_KEY, 0); }
		if (!PlayerPrefs.HasKey(FORCE_BEATING_KEY)) { PlayerPrefs.SetInt(FORCE_BEATING_KEY, 0); }
		if (!PlayerPrefs.HasKey(COINS_KEY)) { PlayerPrefs.SetInt(COINS_KEY, 0); }
		if (!PlayerPrefs.HasKey(SKIN_OWNED_KEY)) { PlayerPrefs.SetString(SKIN_OWNED_KEY, "1000000000"); }
		if (!PlayerPrefs.HasKey(LAST_WATCHED_AD_KEY)) { PlayerPrefs.SetString(LAST_WATCHED_AD_KEY, DateTime.MinValue.ToString(FMT)); }
	}

	public void ResetData(){
		PlayerPrefs.DeleteAll ();
	}

	public void Save(){
		PlayerPrefs.Save ();
	}
}