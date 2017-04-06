#if UNITY_ANDROID
using System;
using GooglePlayGames;
using UnityEngine;
using UnityEngine.SocialPlatforms;

class SocialServicesClientAndroidImpl : SocialServicesClient
{
    public override void SubmitScore(string id, long value, Action<bool> callback)
    {
        Social.ReportScore(value, id, callback);
    }

    public override void SubmitScore(string id, string tag, long value, Action<bool> callback)
    {
        PlayGamesPlatform.Instance.ReportScore(value, id, tag, callback);
    }

    public override void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    public override void ShowLeaderboardUI(string id)
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(id);
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
        PlayGamesPlatform.Instance.IncrementAchievement(id, value, callback);
    }

    public override void LoadAchievement(Action<IAchievement[]> callback)
    {
        PlayGamesPlatform.Instance.LoadAchievements(callback);
    }
}

#endif