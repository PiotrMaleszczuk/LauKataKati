#if UNITY_IOS
using System;
using UnityEngine;
using UnityEngine.Serialization;

class SavedGamesIOSImpl : SavedGamesClient
{
	private const string SAVE_NAME = "ios_save_v1";  
	
	private bool userSignedIn = false;
	
	public override bool IsInitialized() {
		return userSignedIn;
	}
	
    public override void Init(){
		if (OnClientInitialized == null) return;
		OnClientInitialized (userSignedIn = true);
    }

    public override void Save(SaveData data){
		PlayerPrefs.SetString (SAVE_NAME, JsonUtility.ToJson(data));
		PlayerPrefs.Save ();
		if (OnDataSaved == null) return;
		OnDataSaved (true);
    }

	public override void Load(){
		if (OnDataLoaded == null) return;
		var data = JsonUtility.FromJson <SaveData> (PlayerPrefs.GetString (SAVE_NAME));
		if (data == null) {
			OnDataLoaded (new SaveData ());
			return;
		}
		OnDataLoaded (data);
    }
}
#endif