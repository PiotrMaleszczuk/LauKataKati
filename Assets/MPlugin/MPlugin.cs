using UnityEngine;
using UnityEngine.SceneManagement;

public class MPlugin : MonoBehaviour
{
    private const string SCENE_SPLASH = "Splash";

    private static MPlugin instance;
    public static MPlugin Instance
    {
        get { return instance; }
    }

    public static SavedGamesClient Client;
    public static SocialServicesClient SocialServicesClient;

    private SaveData saveData;

    public SaveData SaveData
    {
        get { return this.saveData; }
        set { this.saveData = value; }
    }

    void Awake()
    {
        if (MPlugin.instance == null)
        {
            MPlugin.instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        if (saveData == null)
        {
            saveData = new SaveData();
        }
        InitClient();
    }

    private void InitClient()
    {
        Client = SavedGamesClient.GetInstance();
        SocialServicesClient = SocialServicesClient.GetInstance();
        Client.OnDataLoaded += (data) => {
            this.saveData = data;
            Debug.Log("On Data Loaded");
        };
        Client.OnDataSaved += (success) => {
            Debug.Log("OnDataSaved " + success);
        };
        Client.OnClientInitialized += (success) => {
            Client.Load();
        };

        Client.Init();

    }

    public void ResetData()
    {
        this.saveData = new SaveData();
        Client.Save(Instance.SaveData);
    }

}
