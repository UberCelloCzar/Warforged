using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ConnectionStatus : MonoBehaviour {

    public Text connectionStatus;
	
	// Update is called once per frame
	void Update () {
        // Sets network status to disconnected when a player presses the dc button or leaves the game
        if (!NetworkClient.active && !NetworkServer.active)
            connectionStatus.text = "Disconnected";
    }
}
