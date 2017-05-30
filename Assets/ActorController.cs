using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
	/// <summary>
	/// A very simple object. Moves to the position of the touch with interpolation.
	/// </summary>
	/// 
	public class ActorController : MonoBehaviour {
		private NetworkView _networkView;
		public int turn;
		public bool started = false;

		private void Awake() {
			_networkView = GetComponent<NetworkView>();
		}

		private void Start() {
			turn = FindObjectOfType<TurnController> ().Turn;
			started = true;
		}

		private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
			// Serialize the position and color

			if (started) {
				if (stream.isWriting) {
					int turn = FindObjectOfType<TurnController> ().Turn;
					stream.Serialize (ref turn);

					Debug.Log ("WYSYŁAM TURĘ: " + turn);
				} else {
					int turn = 0;
					stream.Serialize (ref turn);

					Debug.Log ("OTRZYMAŁEM TURĘ: " + turn);
				}
			}
		}
	}
}