using UnityEngine;
using LostPolygon.AndroidBluetoothMultiplayer;
using LostPolygon.AndroidBluetoothMultiplayer.Examples;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class BluetoothController : BluetoothDemoGuiBase
{
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
    protected override void OnDestroy()
    {
        base.OnDestroy();

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
            SetupGame();
        }
        else
        {
            // Show a message if initialization failed for some reason
            Debug.Log("Bluetooth not available. Are you running this on Bluetooth-capable " +
            "Android device and AndroidManifest.xml is set up correctly?");
        }
    }

    private enum SendState
    {
        wait,
        setup,
        transfer
    }
    private SendState sendState = SendState.wait;
    private List<int> xToSend = new List<int>();
    private List<int> yToSend = new List<int>();

    public void SetupGame()
    {
        sendState = SendState.setup;
    }

    public void SendMove(int x, int y)
    {
        print("SendMove: " + x + " "+y);
        sendState = SendState.transfer;
        print("SendMove sendState: " + sendState);
        xToSend.Add(x);
        yToSend.Add(y);
    }


    private void PrepareClick(int x, int y)
    {
        GameObject[] pawnsArray = app.controller.board.PawnsArray;
        bool isPawn = false;
        for (int j = 0; j < pawnsArray.Length; j++)
        {
            PawnScript ps = pawnsArray[j].GetComponent<PawnScript>();
            if (ps.matrix_x == x && ps.matrix_y == y)
            {
                app.controller.board.Click(ps);
                isPawn = true;
                break;
            }
        }
        if(!isPawn)
        {
            GameObject[] empty = app.controller.board.EmptyArray;
            for (int j = 0; j < empty.Length; j++)
            {
                PawnScript ps = empty[j].GetComponent<PawnScript>();
                if (ps.matrix_x == x && ps.matrix_y == y)
                {
                    app.controller.board.Click(ps);
                    break;
                }
            }
        }
    }
    
    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {

        if (started) {
            if (stream.isWriting) {
                print("Writing sendState: " + sendState);
                if (sendState == SendState.setup)
                {
                    int ss = (int)sendState;
                    Debug.Log("Send Sendstate: " + ss);
                    stream.Serialize(ref ss);
                    if (app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_server)
                    {
                        int turn = FindObjectOfType<TurnController>().Turn;
                        Debug.Log("Send turn: " + turn);
                        stream.Serialize(ref turn);
                    }
                    sendState = SendState.wait;
                }
                else if (sendState == SendState.transfer)
                {
                    int ss = (int)sendState;
                    int x = xToSend[0];
                    int y = yToSend[0];
                    Debug.Log("Send Sendstate: " + ss);
                    Debug.Log("Send Move: " + x + " - "+y);

                    stream.Serialize(ref ss);
                    stream.Serialize(ref x);
                    stream.Serialize(ref y);



                    xToSend.RemoveAt(0);
                    yToSend.RemoveAt(0);
                    if (xToSend.Count==0)
                        sendState = SendState.wait;
                }
            } else {
                int ss = (int)SendState.wait;
                stream.Serialize(ref ss);
                Debug.Log("Recive Sendstate: " + ss);
                if (ss == (int)SendState.setup)
                {
                    if (app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_client)
                    {
                        int turn = 0;
                        stream.Serialize(ref turn);
                        Debug.Log("Recive turn: " + turn);
                        app.controller.turns.SetTurnBeforeStart(turn);
                    }
                    else
                    {
                        SetupGame();
                    }
                }
                else if (ss == (int)SendState.transfer)
                {
                    int x = -1;
                    int y = -1;
                    stream.Serialize(ref x);
                    stream.Serialize(ref y);

                    Debug.Log("Recive Move: " + x + " - " + y);
                    PrepareClick(x,y);
                }

            }
		}
	}

    protected override void OnBackToMenu()
    {
        // Gracefully closing all Bluetooth connectivity and loading the menu
        try
        {
            AndroidBluetoothMultiplayer.StopDiscovery();
            AndroidBluetoothMultiplayer.Stop();
        }
        catch
        {
            // 
        }
        SceneManager.LoadScene(SCENE_MENU);

    }

    #region Bluetooth events

    private void OnBluetoothListeningStarted()
    {
        Debug.Log("Event - ListeningStarted");

        // Starting Unity networking server if Bluetooth listening started successfully
        Network.InitializeServer(4, kPort, false);
    }

    private void OnBluetoothListeningStopped()
    {
        Debug.Log("Event - ListeningStopped");

        // For demo simplicity, stop server if listening was canceled
        AndroidBluetoothMultiplayer.Stop();
    }

    private void OnBluetoothDevicePicked(BluetoothDevice device)
    {
        Debug.Log("Event - DevicePicked: " + BluetoothExamplesTools.FormatDevice(device));

        // Trying to connect to a device user had picked
        AndroidBluetoothMultiplayer.Connect(device.Address, kPort);
    }

    private void OnBluetoothClientDisconnected(BluetoothDevice device)
    {
        Debug.Log("Event - ClientDisconnected: " + BluetoothExamplesTools.FormatDevice(device));
    }

    private void OnBluetoothClientConnected(BluetoothDevice device)
    {
        Debug.Log("Event - ClientConnected: " + BluetoothExamplesTools.FormatDevice(device));
    }

    private void OnBluetoothDisconnectedFromServer(BluetoothDevice device)
    {
        Debug.Log("Event - DisconnectedFromServer: " + BluetoothExamplesTools.FormatDevice(device));

        // Stopping Unity networking on Bluetooth failure
        Network.Disconnect();
    }

    private void OnBluetoothConnectionToServerFailed(BluetoothDevice device)
    {
        Debug.Log("Event - ConnectionToServerFailed: " + BluetoothExamplesTools.FormatDevice(device));
    }

    private void OnBluetoothConnectedToServer(BluetoothDevice device)
    {
        Debug.Log("Event - ConnectedToServer: " + BluetoothExamplesTools.FormatDevice(device));

        // Trying to negotiate a Unity networking connection, 
        // when Bluetooth client connected successfully
        Network.Connect(kLocalIp, kPort);
    }

    private void OnBluetoothAdapterDisabled()
    {
        Debug.Log("Event - AdapterDisabled");
    }

    private void OnBluetoothAdapterEnableFailed()
    {
        Debug.Log("Event - AdapterEnableFailed");
    }

    private void OnBluetoothAdapterEnabled()
    {
        Debug.Log("Event - AdapterEnabled");

        // Resuming desired action after enabling the adapter
        switch (_desiredMode)
        {
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

    private void OnBluetoothDiscoverabilityEnableFailed()
    {
        Debug.Log("Event - DiscoverabilityEnableFailed");
    }

    private void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration)
    {
        Debug.Log(string.Format("Event - DiscoverabilityEnabled: {0} seconds", discoverabilityDuration));
    }

    #endregion Bluetooth events

}