using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CardNetworkManager : NetworkManager
{

    const int maxPlayers = 2;
    Player[] playerSlots = new Player[2];

    /* Not sure if starting at 0 or 1 for some reason */
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) // When the server tries to add another connection
    {
        for (int slot = 1; slot < maxPlayers + 1; slot++) // See if there's a slot available
        {
            if (playerSlots[slot] == null)
            {
                var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity); // Create the new player if there's space
                var player = playerObj.GetComponent<Player>(); // Get the player script to do stuff with it

                player.playerId = slot; // The player ID is the same as the slot number
                playerSlots[slot] = player; // Add the player to the slot

                Debug.Log("Adding player in slot " + slot);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId); // Associate the player with the requesting connection
                return;
            }
        }
        
        conn.Disconnect(); // If no slot, DC. ***HERE IS WHERE A "LOBBY FULL" KICKBACK GOES***
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) // When someone commands to be removed
    {
        var player = playerController.gameObject.GetComponent<Player>(); // Grab the player script from the DCer
        playerSlots[player.playerId] = null; // Scrap that player in the list
        WarManager.singleton.RemovePlayer(player); // Have the game manager remove the player and handle DC

        base.OnServerRemovePlayer(conn, playerController); // Do the base connection break stuff
    }

    public override void OnServerDisconnect(NetworkConnection conn) // This is called when a client disconnects
    {
        foreach (var playerController in conn.playerControllers)
        {
            var player = playerController.gameObject.GetComponent<Player>(); // Grab that player's script
            playerSlots[player.playerId] = null; // Scrap that player in the list
            WarManager.singleton.RemovePlayer(player); // Call to remove the player
        }

        base.OnServerDisconnect(conn); // Do the base connection break
    }

    public override void OnStartClient(NetworkClient client) // When a client starts up
    {
        //client.RegisterHandler(1000, OnCardMsg); // Not 100% sure what this does
    }

    //void OnCardMsg(NetworkMessage netMsg) // Not sure what this does
    //{
    //    var msg = netMsg.ReadMessage<CardConstants.CardMessage>();

    //    var other = ClientScene.FindLocalObject(msg.playerId);
    //    var player = other.GetComponent<Player>();
    //    player.MsgAddCard(msg.cardId);
    //}

}
