using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchController : MonoBehaviour
{
    #region Variables
    public bool IsReady { get { return localPlayer != null && remotePlayer != null; } }
    public PlayerController localPlayer; // The iteration of the script being run on the machine the user is on
    public PlayerController remotePlayer; // The iteration of the script being run on the opponents machine
    public PlayerController hostPlayer; // THe player hosting the game
    public PlayerController clientPlayer; // The client player
    public List<PlayerController> players = new List<PlayerController>();
    #endregion

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject); // Immortalize object
    }

    #region Event Handlers
    public void OnPlayerStarted(object sender, object args)
    {
        // Adds player to players list and checks if the game can begin
        players.Add((PlayerController)sender);
        Configure();
    }

    public void OnPlayerStartedLocal(object sender, object args)
    {
        // Sets the local player to the iteration of this script.
        localPlayer = (PlayerController)sender;
        Configure();
    }

    public void OnPlayerDestroyed(object sender, object args)
    {
        // Resets variables on disconnect so they can be reused without reopening game.
        PlayerController pc = (PlayerController)sender;
        if (localPlayer == pc)
            localPlayer = null;
        if (remotePlayer == pc)
            remotePlayer = null;
        if (hostPlayer == pc)
            hostPlayer = null;
        if (clientPlayer == pc)
            clientPlayer = null;
        if (players.Contains(pc))
            players.Remove(pc);
    }

    void Configure()
    {
        // This method determines wether or not 2 players have connected, if they have it sets the local and remote player
        // And shows the machines are connected on screen.

        if (localPlayer == null || players.Count < 2)
            return;

        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i] != localPlayer)
            {
                remotePlayer = players[i];
                break;
            }
        }

        hostPlayer = (localPlayer.isServer) ? localPlayer : remotePlayer; // If isSer then lP if not then rP
        clientPlayer = (localPlayer.isServer) ? remotePlayer : localPlayer;

        localPlayer.CmdConnectionStatus("Connected!");
    }
    #endregion
}