using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

class SavedGamesEditorImpl : SavedGamesClient {
    private const string PP_KEY = "saved_game";
    private XmlSerializer serializer;

    private bool userSignedIn;

    public SavedGamesEditorImpl() {
        this.serializer = new XmlSerializer(typeof(SaveData));
    }

    public override void Init() {
        if (OnClientInitialized != null) {
            userSignedIn = true;
            OnClientInitialized(userSignedIn);
        }
    }

    public override void Save(SaveData data) {
        StringWriter stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, data);
        PlayerPrefs.SetString(PP_KEY, stringWriter.ToString());
        PlayerPrefs.Save();
        if (OnDataSaved != null) OnDataSaved(true);
    }

    public override void Load() {
        if (OnDataLoaded == null) return;
        try {
            StringReader stringReader = new StringReader(PlayerPrefs.GetString(PP_KEY));
            OnDataLoaded((SaveData) serializer.Deserialize(stringReader));
            return;
        } catch (Exception) {
            // ignored
        }
        OnDataLoaded(new SaveData());
    }

    public override bool IsInitialized() {
        return userSignedIn;
    }
}