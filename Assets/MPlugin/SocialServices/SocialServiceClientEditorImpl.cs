using System;
using System.Diagnostics;
using UnityEngine.SocialPlatforms;
using Debug = UnityEngine.Debug;

class SocialServiceClientEditorImpl : SocialServicesClient
{
    public const string TAG = "SocialServiceClientEditorImpl";

    public override void SubmitScore(string id, long value, Action<bool> callback)
    {
        log(string.Format("#SubmitScore({0}, {1}, {2}", id, value, callback));
        if (callback != null) callback(true);
    }

    public override void SubmitScore(string id, string tag, long value, Action<bool> callback)
    {
        log(string.Format("#SubmitScore({0}, {1}, {2}, {3}", id, tag,  value, callback));
        if (callback != null) callback(true);
    }

    public override void ShowLeaderboardUI()
    {
        log("#ShowLeaderboardUI()");
    }

    public override void ShowLeaderboardUI(string id)
    {
        log(string.Format("#ShowLeaderboardUI({0}", id));
    }

    public override void ShowAchievementUI()
    {
        log("#ShowAchievementUI()");
    }

    public override void UnlockAchievement(string id, float percentage, Action<bool> callback)
    {
        log(string.Format("#UnlockAchievement({0}, {1}, {2})", id, percentage, callback));
        if (callback != null) callback(true);
    }

    public override void IncrementAchievement(string id, int value, Action<bool> callback)
    {
        log(string.Format("#IncrementAchievement({0}, {1}, {2})", id, value, callback));
        if (callback != null) callback(true);
    }

    public override void LoadAchievement(Action<IAchievement[]> callback)
    {
        log(string.Format("#LoadAchievement({0}", callback));
        if (callback != null) callback(new IAchievement[]{});
    }

    private void log(string msg)
    {
        Debug.Log(string.Format("{0}: {1}", TAG, msg));
    }
}