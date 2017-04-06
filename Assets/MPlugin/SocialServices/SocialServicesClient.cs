using System;
using UnityEngine.SocialPlatforms;

public abstract class SocialServicesClient
{
    public static SocialServicesClient GetInstance()
    {
#if UNITY_EDITOR
        return new SocialServiceClientEditorImpl();
#elif UNITY_ANDROID
        return new SocialServicesClientAndroidImpl();
#elif UNITY_IOS
        return new SocialServicesClientIOSImpl();
#endif
    }

    public abstract void SubmitScore(string id, long value, Action<bool> callback);
    public abstract void SubmitScore(string id, string tag, long value, Action<bool> callback);
    public abstract void ShowLeaderboardUI();
    public abstract void ShowLeaderboardUI(string id);

    public abstract void ShowAchievementUI();
    public abstract void UnlockAchievement(string id, float percentage, Action<bool> callback);
    public abstract void IncrementAchievement(string id, int value, Action<bool> callback);
    public abstract void LoadAchievement(Action<IAchievement[]> callback);
}