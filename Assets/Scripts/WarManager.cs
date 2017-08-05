using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WarManager : NetworkBehaviour {

    public static WarManager singleton;
    public Player localPlayer;

    public List<Player> players = new List<Player>();

    private void Awake()
    {
        singleton = this;
    }

    public void RemovePlayer(Player player) // Remove a player
    {
        players.Remove(player); // **HANDLE DC NOTIFICATION HERE**
    }
}
