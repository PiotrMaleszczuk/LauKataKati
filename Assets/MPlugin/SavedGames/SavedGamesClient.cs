using System;

public abstract class SavedGamesClient
{
    public static SavedGamesClient GetInstance()
    {
#if UNITY_EDITOR
        return new SavedGamesEditorImpl();
#elif UNITY_ANDROID
        return new SavedGamesAndroidImpl();
#elif UNITY_IOS
        return new SavedGamesIOSImpl();
#endif
    }

    /// <summary>
    /// Called after client initialization
    /// </summary>
    public Action<bool> OnClientInitialized;

    /// <summary>
    /// called after data was saved, arg value true indicates success
    /// </summary>
    public Action<bool> OnDataSaved;

    /// <summary>
    /// called after data was loaded, arg value null if can`t load data
    /// </summary>
    public Action<SaveData> OnDataLoaded;

    /// <summary>
    /// Should be called once.
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// Used for saving data
    /// </summary>
    /// <param name="data">data to be saved</param>
    public abstract void Save(SaveData data);

    /// <summary>
    /// Used for loading data
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// Returns true if client is initialized
    /// </summary>
    /// <returns></returns>
    public abstract bool IsInitialized();
}
