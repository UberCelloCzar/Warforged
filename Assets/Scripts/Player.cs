using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour { // Any variable marked "SyncVar" is set and held by the server. I think.

    [SyncVar]
    public int playerId; // The player's ID for reference. This is set and held by the server.

    public override void OnStartLocalPlayer()
    {
        WarManager.singleton.localPlayer = this;
    }
    
}
