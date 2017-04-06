#if UNITY_ANDROID
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

class SavedGamesAndroidImpl : SavedGamesClient
{
    public const string TAG = "SavedGamesAndroidImpl";
    public const bool DEBUG = true;
    private const string SAVE_NAME = "android_save";

    private bool userSignedIn;

    public override void Init()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignInUser();
    }

    public override void Save(SaveData data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            SAVE_NAME,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (openStatus, openedGame) =>
            {
                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                builder = builder
                    .WithUpdatedPlayedTime(TimeSpan.FromMilliseconds(DateTime.Now.Millisecond))
                    .WithUpdatedDescription("Saved game at " + DateTime.Now);
                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                savedGameClient.CommitUpdate(openedGame, updatedMetadata, Serialize(data), (status, game) =>
                {
                    FireOnGameSavedEvent(status == SavedGameRequestStatus.Success);
                });
            }
        );
    }

    private byte[] Serialize(SaveData data)
    {
        if (data == null) return new byte[0];
        BinaryFormatter binFormatter = new BinaryFormatter();
        MemoryStream mStream = new MemoryStream();

        binFormatter.Serialize(mStream, data);
        return mStream.ToArray();
    }

    private SaveData Deserialize(byte[] data)
    {
         try
        {
            if (data == null || data.Length == 0) return new SaveData();
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter binFormatter = new BinaryFormatter();

            mStream.Write(data, 0, data.Length);
            mStream.Position = 0;

            return binFormatter.Deserialize(mStream) as SaveData;
        }
        catch (SerializationException)
        {
            return new SaveData();
        }
    }

    private void FireOnGameSavedEvent(bool success)
    {
        if (OnDataSaved == null) return;
        OnDataSaved(success);
    }

    public override void Load()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            SAVE_NAME,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (openStatus, game) =>
            {
                log(string.Format("Load() - openStatus={0}, game={1}", openStatus, game));
                if (game == null) return;

                savedGameClient.ReadBinaryData(game, (status, data) =>
                {
                    FireOnDataReadCallback(status == SavedGameRequestStatus.Success ? data : null);
                });
            }
        );
        
    }

    public override bool IsInitialized() {
        return userSignedIn;
    }

    private void FireOnSignInCallback(bool val)
    {
        if (OnClientInitialized == null) return;
        OnClientInitialized(val);
    }

    private void FireOnDataReadCallback(byte[] data)
    {
        if (OnDataLoaded == null) return;
        OnDataLoaded(Deserialize(data));
    }
    
    // Sign in the user
    public void SignInUser()
    {
        if (!userSignedIn)
        {
            PlayGamesPlatform.Instance.Authenticate(CallbackSignInUser);
        }
    }

    // The sign in callback
    void CallbackSignInUser(bool success)
    {
        if (success)
        {
            userSignedIn = true;
        }
        FireOnSignInCallback(success);
    }

    private void log(string msg)
    {
        if (DEBUG)
        {
            Debug.Log(string.Format("{0}:\t{1}", TAG, msg));
        }
    }

}
#endif