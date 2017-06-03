using UnityEngine;
using System.Collections.Generic;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
	public class ActorController : MonoBehaviour {
		private App app;
		private NetworkView _networkView;
		public bool started = false;

		private void Awake() {
			_networkView = GetComponent<NetworkView>();
		}

		private void Start() {
			app = App.Instance;
			this.isServer = app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_server ? true : false;
			started = true;
		}

		private bool isServer = false;
		// 1 - setup, 2 - waiting, 3 - transfer
		private int sendState {
			get { return app.controller.gameMode.sendState; }
			set { app.controller.gameMode.sendState = value; }
		}

		private List<int> xToSend {
			get { return app.controller.gameMode.xToSend; }
			set { app.controller.gameMode.xToSend = value; }
		}

		private List<int> yToSend {
			get { return app.controller.gameMode.yToSend; }
			set { app.controller.gameMode.yToSend = value; }
		}

		public void SetupGame()
		{
			//state 1 - waiting
			sendState = 1;
		}

		public void SendMove(int x, int y)
		{
			print("SendMove: " + x + " "+y);
			//state 3 - transfer
			sendState = 3;
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

		private void ChangeState(int state) {
			this.sendState = state;
		}

		private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
			if (started) {
				if (stream.isWriting) {
					int ss = 4;
					int turn = 4;
					int x = 9;
					int y = 9;

					if (sendState == 1 && isServer) {
						ss = sendState;
						turn = app.controller.turns.Turn;
						Debug.Log ("Serwer Wysyłanie - SetUp: " + "State: " + ss + ", Turn " + turn + ", x, y :" + x + ", " + y);
					} else if (sendState == 1 && !isServer) {
						Debug.Log ("Client Wysyłanie - SetUp: Waiting for setup, moj stan: " + sendState);
					} else if (sendState == 2 && !isServer) {
						Debug.Log ("Client Wysyłanie - Waiting, moj stan: " + sendState);
						ss = sendState;
					} else if (sendState == 2 && isServer) {
						Debug.Log ("Serwer Wysyłanie - Waiting, moj stan: " + sendState);
						ss = sendState;
					} else if (sendState == 3){
						ss = sendState;
						x = xToSend[0];
						y = yToSend[0];
						Debug.Log("Send Sendstate: " + ss+" mode: "+app.controller.gameMode.mode);
						Debug.Log("Send Move: " + x + " - "+y+" mode: "+app.controller.gameMode.mode);
						xToSend.RemoveAt(0);
						yToSend.RemoveAt(0);
						if (xToSend.Count==0)
							sendState = 2;
					}

					string all = ss.ToString () + turn.ToString () + x.ToString () + y.ToString ();
					int toSend = int.Parse (all);
					Debug.Log ("Do wysłania (klient/serwer): " + toSend);
					stream.Serialize (ref toSend);

				} else {
					int recived = 4499;
					stream.Serialize (ref recived);
					Debug.Log ("Odbieranie (Klient/Serwer): " + recived);
					string all = recived.ToString ();
					Debug.Log ("Odbieranie (Klient/Serwer): (string) " + all);
					int ss = int.Parse (all [0].ToString());
					int turn = int.Parse (all [1].ToString());
					int x = int.Parse (all [2].ToString());
					int y = int.Parse (all [3].ToString());

					Debug.Log ("Odbieranie (Klient/Serwer): " + "State: " + ss + ", Turn " + turn + ", x, y :" + x + ", " + y);
					if (isServer && sendState == 1 && ss == 2) {
						Debug.Log ("Serwer Odbieranie - SetUp: " + " Potwierdzenie ustawienia tury klienta");
						app.controller.bluetooth.ServerClientMultiplayerWindow.SetActive (false);
						this.ChangeState (2);
					} else if (!isServer && sendState == 1 && ss == 1) {
						app.controller.turns.SetTurnBeforeStart (turn);
						app.controller.bluetooth.ServerClientMultiplayerWindow.SetActive (false);
						Debug.Log ("Client Odbieranie - SetUp: " + "Ustawiam ture na " + turn);
						Debug.Log ("Stan przed ustawieniem: " + sendState);
						Debug.Log ("Ustawiam na waiting (2)");
						this.ChangeState (2);
						Debug.Log ("Stan po ustawieniu: " + sendState);
					} else if (ss == 3) {
						Debug.Log("Recive Move: " + x + " - " + y+" mode: "+app.controller.gameMode.mode);
						PrepareClick(x,y);
					}
				}
			}

		}
	}
}