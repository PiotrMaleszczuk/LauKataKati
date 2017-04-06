#if UNITY_IOS 
 
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

class SocialServicesClientIOSImpl : SocialServicesClient
{
	public SocialServicesClientIOSImpl(){
		Social.localUser.Authenticate (ProcessAuthentication);
	}

	void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("Authenticated, checking achievements");
		}  else {
			Debug.Log ("Failed to authenticate");
		}      
	}

	public override void SubmitScore(string id, long value, Action<bool> callback)
    {
        Social.ReportScore(value, id, callback);
    }

    public override void SubmitScore(string id, string tag, long value, Action<bool> callback)
    {
		Social.ReportScore(value, id, callback);
		Debug.LogWarning ("highscore tag is not supported in iOS");
    }

    public override void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    public override void ShowLeaderboardUI(string id)
    {
		Social.ShowLeaderboardUI ();
    }

    public override void ShowAchievementUI()
    {
        Social.ShowAchievementsUI();
    }

    public override void UnlockAchievement(string id, float percentage, Action<bool> callback)
    {
        Social.ReportProgress(id, percentage, callback);
    }

    public override void IncrementAchievement(string id, int value, Action<bool> callback)
    {
		throw new NotImplementedException("Not supported by iOS");
    }

    public override void LoadAchievement(Action<IAchievement[]> callback)
	{
		Social.LoadAchievements (callback);
    }
}
#endif
