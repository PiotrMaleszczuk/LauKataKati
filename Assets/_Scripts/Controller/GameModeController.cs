using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour {

    public Mode mode { get; private set; }

    public enum Mode
    {
        single,
        multiplayer_local,
        multiplayer_bluetooth_client,
        multiplayer_bluetooth_server
    }

	public int sendState = 1;

	public List<int> xToSend = new List<int>();
	public List<int> yToSend = new List<int>();

    // Use this for initialization
    public void Init () {
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
            default:
                this.mode = Mode.single;
                break;
        }
    }
	
}
