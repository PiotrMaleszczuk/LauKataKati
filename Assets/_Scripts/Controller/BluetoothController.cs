using UnityEngine;
using LostPolygon.AndroidBluetoothMultiplayer;
using LostPolygon.AndroidBluetoothMultiplayer.Examples;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class BluetoothController : MonoBehaviour
{
	public GameObject ActorPrefab; // Reference to the test actor
    private App app;

    private const string SCENE_MENU = "Menu";

    private Mode mode;

    private enum Mode
    {
        single,
        multiplayer_local,
        multiplayer_bluetooth_client,
        multiplayer_bluetooth_server
    }
    private NetworkView _networkView;

    private const string kLocalIp = "127.0.0.1"; // An IP for Network.Connect(), must always be 127.0.0.1
    private const int kPort = 28000; // Local server IP. Must be the same for client and server
    private bool started = false;
    private bool _initResult;
    private BluetoothMultiplayerMode _desiredMode = BluetoothMultiplayerMode.None;


    // Don't forget to unregister the event delegates!
    void OnDestroy()
	{
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
        AndroidBluetoothMultiplayer.ListeningStarted -= OnBluetoothListeningStarted;
        AndroidBluetoothMultiplayer.ListeningStopped -= OnBluetoothListeningStopped;
        AndroidBluetoothMultiplayer.AdapterEnabled -= OnBluetoothAdapterEnabled;
        AndroidBluetoothMultiplayer.AdapterEnableFailed -= OnBluetoothAdapterEnableFailed;
        AndroidBluetoothMultiplayer.AdapterDisabled -= OnBluetoothAdapterDisabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnabled -= OnBluetoothDiscoverabilityEnabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed -= OnBluetoothDiscoverabilityEnableFailed;
        AndroidBluetoothMultiplayer.ConnectedToServer -= OnBluetoothConnectedToServer;
        AndroidBluetoothMultiplayer.ConnectionToServerFailed -= OnBluetoothConnectionToServerFailed;
        AndroidBluetoothMultiplayer.DisconnectedFromServer -= OnBluetoothDisconnectedFromServer;
        AndroidBluetoothMultiplayer.ClientConnected -= OnBluetoothClientConnected;
        AndroidBluetoothMultiplayer.ClientDisconnected -= OnBluetoothClientDisconnected;
        AndroidBluetoothMultiplayer.DevicePicked -= OnBluetoothDevicePicked;
    }

    public void Init()
    {
        app = App.Instance;

        // Setting the UUID. Must be unique for every application
        _initResult = AndroidBluetoothMultiplayer.Initialize("8ce255c0-200a-11e0-ac64-0800200c9a66");

        // Enabling verbose logging. See log cat!
        AndroidBluetoothMultiplayer.SetVerboseLog(true);

        // Registering the event delegates
        AndroidBluetoothMultiplayer.ListeningStarted += OnBluetoothListeningStarted;
        AndroidBluetoothMultiplayer.ListeningStopped += OnBluetoothListeningStopped;
        AndroidBluetoothMultiplayer.AdapterEnabled += OnBluetoothAdapterEnabled;
        AndroidBluetoothMultiplayer.AdapterEnableFailed += OnBluetoothAdapterEnableFailed;
        AndroidBluetoothMultiplayer.AdapterDisabled += OnBluetoothAdapterDisabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnabled += OnBluetoothDiscoverabilityEnabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed += OnBluetoothDiscoverabilityEnableFailed;
        AndroidBluetoothMultiplayer.ConnectedToServer += OnBluetoothConnectedToServer;
        AndroidBluetoothMultiplayer.ConnectionToServerFailed += OnBluetoothConnectionToServerFailed;
        AndroidBluetoothMultiplayer.DisconnectedFromServer += OnBluetoothDisconnectedFromServer;
        AndroidBluetoothMultiplayer.ClientConnected += OnBluetoothClientConnected;
        AndroidBluetoothMultiplayer.ClientDisconnected += OnBluetoothClientDisconnected;
        AndroidBluetoothMultiplayer.DevicePicked += OnBluetoothDevicePicked;

        _networkView = GetComponent<NetworkView>();
        int modeIndex = SaveDataController.Instance.Data.mode;
        switch (modeIndex)
        {
            case 1:
                this.mode = Mode.single;
                break;
            case 2:
                this.mode = Mode.multiplayer_local;
                break;
            case 3:
                this.mode = Mode.multiplayer_bluetooth_server;
                break;
            case 4:
                this.mode = Mode.multiplayer_bluetooth_client;
                break;
        }

        float scaleFactor = BluetoothExamplesTools.UpdateScaleMobile();
        // If initialization was successfull, showing the buttons
        if (_initResult)
        {
            // If there is no current Bluetooth connectivity
            BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
            if (currentMode == BluetoothMultiplayerMode.None)
            {
                if (mode == Mode.multiplayer_bluetooth_server)
                {
                    // If Bluetooth is enabled, then we can do something right on
                    if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled())
                    {
                        AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
                        Network.Disconnect(); // Just to be sure
                        AndroidBluetoothMultiplayer.StartServer(kPort);
                    }
                    else
                    {
                        // Otherwise we have to enable Bluetooth first and wait for callback
                        _desiredMode = BluetoothMultiplayerMode.Server;
                        AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
                    }
                    Debug.Log("Server");
                }

                if (mode == Mode.multiplayer_bluetooth_client)
                {
                    // If Bluetooth is enabled, then we can do something right on
                    if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled())
                    {
                        Network.Disconnect(); // Just to be sure
                        AndroidBluetoothMultiplayer.ShowDeviceList(); // Open device picker dialog
                    }
                    else
                    {
                        // Otherwise we have to enable Bluetooth first and wait for callback
                        _desiredMode = BluetoothMultiplayerMode.Client;
                        AndroidBluetoothMultiplayer.RequestEnableBluetooth();
                    }
                    Debug.Log("Client");
                }
            }
            else
            {
                Debug.Log("Bluetooth activated before Init");
            }
            /*if (currentMode == BluetoothMultiplayerMode.None)
            {
                OnBackToMenu();
            }
            else
            {
                started = true;
            }*/
            started = true;
            //SetupGame();
        }
        else
        {
            // Show a message if initialization failed for some reason
            Debug.Log("Bluetooth not available. Are you running this on Bluetooth-capable " +
            "Android device and AndroidManifest.xml is set up correctly?");
        }
    }

    

	private void OnGUI() {
		BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
		if (currentMode != BluetoothMultiplayerMode.None) {
			GUI.contentColor = Color.black;
			GUI.Label (
				new Rect (10, 10, 300, 100), 
				currentMode == BluetoothMultiplayerMode.Client ? "Client Connected" : "Server Connected");
		} else {
			GUI.contentColor = Color.black;
			GUI.Label (
				new Rect (10, 10, 300, 100), 
				"Disconnected");
		}
	}
    
    

	public void StopConnection(){
		if (Network.peerType != NetworkPeerType.Disconnected)
			Network.Disconnect();
		OnBackToMenu ();
	}

	public void OnBackToMenu() {
		// Gracefully closing all Bluetooth connectivity and loading the menu
		try {
			AndroidBluetoothMultiplayer.StopDiscovery();
			AndroidBluetoothMultiplayer.Stop();
		} catch {
			// 
		}
	}

	#region Bluetooth events

	private void OnBluetoothListeningStarted() {
		Debug.Log("Event - ListeningStarted");

		// Starting Unity networking server if Bluetooth listening started successfully
		Network.InitializeServer(4, kPort, false);
	}

	private void OnBluetoothListeningStopped() {
		Debug.Log("Event - ListeningStopped");

		// For demo simplicity, stop server if listening was canceled
		AndroidBluetoothMultiplayer.Stop();
	}

	private void OnBluetoothDevicePicked(BluetoothDevice device) {
		Debug.Log("Event - DevicePicked: " + BluetoothExamplesTools.FormatDevice(device));

		// Trying to connect to a device user had picked
		AndroidBluetoothMultiplayer.Connect(device.Address, kPort);
	}

	private void OnBluetoothClientDisconnected(BluetoothDevice device) {
		Debug.Log("Event - ClientDisconnected: " + BluetoothExamplesTools.FormatDevice(device));
	}

	private void OnBluetoothClientConnected(BluetoothDevice device) {
		Debug.Log("Event - ClientConnected: " + BluetoothExamplesTools.FormatDevice(device));
	}

	private void OnBluetoothDisconnectedFromServer(BluetoothDevice device) {
		Debug.Log("Event - DisconnectedFromServer: " + BluetoothExamplesTools.FormatDevice(device));

		// Stopping Unity networking on Bluetooth failure
		Network.Disconnect();
	}

	private void OnBluetoothConnectionToServerFailed(BluetoothDevice device) {
		Debug.Log("Event - ConnectionToServerFailed: " + BluetoothExamplesTools.FormatDevice(device));
	}

	private void OnBluetoothConnectedToServer(BluetoothDevice device) {
		Debug.Log("Event - ConnectedToServer: " + BluetoothExamplesTools.FormatDevice(device));

		// Trying to negotiate a Unity networking connection, 
		// when Bluetooth client connected successfully
		Network.Connect(kLocalIp, kPort);
	}

	private void OnBluetoothAdapterDisabled() {
		Debug.Log("Event - AdapterDisabled");
	}

	private void OnBluetoothAdapterEnableFailed() {
		Debug.Log("Event - AdapterEnableFailed");
	}

	private void OnBluetoothAdapterEnabled() {
		Debug.Log("Event - AdapterEnabled");

		// Resuming desired action after enabling the adapter
		switch (_desiredMode) {
		case BluetoothMultiplayerMode.Server:
			Network.Disconnect();
			AndroidBluetoothMultiplayer.StartServer(kPort);
			break;
		case BluetoothMultiplayerMode.Client:
			Network.Disconnect();
			AndroidBluetoothMultiplayer.ShowDeviceList();
			break;
		}

		_desiredMode = BluetoothMultiplayerMode.None;
	}

	private void OnBluetoothDiscoverabilityEnableFailed() {
		Debug.Log("Event - DiscoverabilityEnableFailed");
	}

	private void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration) {
		Debug.Log(string.Format("Event - DiscoverabilityEnabled: {0} seconds", discoverabilityDuration));
	}

	#endregion Bluetooth events

	#region Network events

	private void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player disconnected: " + player.GetHashCode());
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	private void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Can't connect to the networking server");

		// Stopping all Bluetooth connectivity on Unity networking disconnect event
		AndroidBluetoothMultiplayer.Stop();
	}

	private void OnDisconnectedFromServer() {
		Debug.Log("Disconnected from server");

		// Stopping all Bluetooth connectivity on Unity networking disconnect event
		AndroidBluetoothMultiplayer.Stop();

		TestActor[] objects = FindObjectsOfType(typeof(TestActor)) as TestActor[];
		if (objects != null) {
			foreach (TestActor obj in objects) {
				Destroy(obj.gameObject);
			}
		}
	}

	private void OnConnectedToServer() {
		Debug.Log("Connected to server");

		// Instantiating a simple test actor
		Network.Instantiate(ActorPrefab, Vector3.zero, Quaternion.identity, 0);
	}

	private void OnServerInitialized() {
		Debug.Log("Server initialized");

		// Instantiating a simple test actor
		if (Network.isServer) {
			Network.Instantiate(ActorPrefab, Vector3.zero, Quaternion.identity, 0);
		}
	}

	#endregion Network events

}