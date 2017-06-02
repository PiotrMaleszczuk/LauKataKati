using UnityEngine;
using System.Collections.Generic;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
	/// <summary>
	/// A very simple object. Moves to the position of the touch with interpolation.
	/// </summary>
	/// 
	public class ActorController : MonoBehaviour {
		private App app;
		private NetworkView _networkView;
		//public int turn = 0;
		public bool started = false;

		private void Awake() {
			_networkView = GetComponent<NetworkView>();
		}

		private void Start() {
			app = App.Instance;
			//turn = FindObjectOfType<TurnController> ().Turn;
			started = true;
			if (app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_server)
				sendState = SendState.setup;
			else
				sendState = SendState.wait;
		}

		private enum SendState
		{
			wait,
			setup,
			transfer
		}
		private SendState sendState;
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

//			if (stream.isWriting) {
//				float liczba = 0f;
//				if (app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_client) {
//					liczba = 100f;
//				} else {
//					liczba = 50f;
//				}
//
//				stream.Serialize(ref liczba);
//				Debug.Log ("Send: " + liczba);
//			} else {
//				float liczba = 0f;
//
//				stream.Serialize(ref liczba);
//				Debug.Log ("Recive: " + liczba);
//			}
//

			if (stream.isWriting) {
				//print("Writing sendState: " + sendState);
				if (sendState == SendState.setup && app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_server)
				{
					print("przeszlo ifa writing"+" mode: "+app.controller.gameMode.mode);
					int ss = (int)sendState;
					Debug.Log("Send Sendstate: " + ss+" mode: "+app.controller.gameMode.mode);
					stream.Serialize(ref ss);
					print ("przeszlo ifa writing serwera"+" mode: "+app.controller.gameMode.mode);
					int turn = FindObjectOfType<TurnController>().Turn;
					Debug.Log("Send turn: " + turn+" mode: "+app.controller.gameMode.mode);
					stream.Serialize(ref turn);
					sendState = SendState.wait;
				}
				else if (sendState == SendState.transfer)
				{
					print ("przeszlo ifa transfer"+" mode: "+app.controller.gameMode.mode);
					int ss = (int)sendState;
					int x = xToSend[0];
					int y = yToSend[0];
					Debug.Log("Send Sendstate: " + ss+" mode: "+app.controller.gameMode.mode);
					Debug.Log("Send Move: " + x + " - "+y+" mode: "+app.controller.gameMode.mode);

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
				Debug.Log("Recive Sendstate: " + ss +" mode: "+app.controller.gameMode.mode);
				if (ss == (int)SendState.setup)
				{
					print ("recive/if setup"+" mode: "+app.controller.gameMode.mode);
					if (app.controller.gameMode.mode == GameModeController.Mode.multiplayer_bluetooth_client)
					{
						print ("if client"+" mode: "+app.controller.gameMode.mode);
						int turn = 0;
						stream.Serialize(ref turn);
						Debug.Log("Recive turn: " + turn);
						app.controller.turns.SetTurnBeforeStart(turn);
					}
				}
				else if (ss == (int)SendState.transfer)
				{
					print ("przeszlo ifa odbior transfer"+" mode: "+app.controller.gameMode.mode);
					int x = -1;
					int y = -1;
					stream.Serialize(ref x);
					stream.Serialize(ref y);

					Debug.Log("Recive Move: " + x + " - " + y+" mode: "+app.controller.gameMode.mode);
					PrepareClick(x,y);
				}

			}
			
		}
	}
}