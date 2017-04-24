using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Todo: Disable buttons when they are no longer functional or enabled at inoprtune moments.
    // Todo: Consider UI revamp when we get more players (or if we switch to server)

    #region Variables
    public Text connectionStatus;
    public Text newName;

    private InputField ipAddressfield;
    private NetworkManager manager;
    private MatchController controller;
    #endregion

    void Awake()
    {
        // Sets controller and network manager
        manager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        controller = GameObject.Find("Match Controller").GetComponent<MatchController>();
        try //Used a try block as I used this script for the char select screen which didnt have object inside, was too lazy to make everything public, will do later lmao
        {
            ipAddressfield = GameObject.Find("IP Address Field").GetComponent<InputField>();
        }
        catch (System.NullReferenceException)
        {}
        
    }

    public void changeName()
    {
        // Changes username. Note that this only changes it locally, as there isn't a need to do it remotely
        controller.localPlayer.playerName = newName.text;
    }

    public void hostMatch()
    {
        manager.StartHost();
        connectionStatus.text = "Waiting for Player...";
    }

    public void connectToClient()
    {
        if (ipAddressfield.text != "")
            manager.networkAddress = ipAddressfield.text;
        else
            manager.networkAddress = "localhost"; // localhost is 127.0.0.1 for those who don't know
        manager.StartClient();
        connectionStatus.text = "Attempting to Connect...";
    }

    public void disconnect()
    {
        connectionStatus.text = "Disconnected";
        manager.StopHost();
    }

    public void connectToPlayer(string playerName)
    {
        // Just add more const Ip's as we get more people until we need a server.
        
        const string ownIP = "127.0.0.1";

        switch (playerName)
        {
            case "own":
                ipAddressfield.text = ownIP;
                manager.networkAddress = ownIP;
                break;
        }

        connectToClient();
    }

    public void startMatch()
    {
        /*
        An example of the ready flag in action. First it checks if the remote is ready. If he's ready we don't have
        to set anything so we can just continue, in this case that means moving to the character selection screen. If
        the remote player isn't ready, we call the Cmd ready method. That way when the remote player is ready and calls
        it, it will check if the remote player (which is now us) is ready, which we are. And it will run the next cmd
        scene loader to continue.
        */
        if (controller.remotePlayer.readyFlag == true)
        {
            controller.remotePlayer.readyFlag = false;
            controller.localPlayer.readyFlag = false;
            controller.localPlayer.CmdMoveToScene("WarforgedCharacterSelect");
        }          
        else
        {
            controller.localPlayer.CmdImReady(controller.localPlayer.isServer);
            
            connectionStatus.text = "Waiting to begin...";
        }
    }

    public void selectDeck()
    {
        // This one is honestly so similar it isn't really worth explaining again.
        if (controller.remotePlayer.readyFlag == true)
        {
            controller.remotePlayer.readyFlag = false;
            controller.localPlayer.readyFlag = false;
            controller.localPlayer.CmdMoveToScene("WarforgedBoard");

            if (StartGame.characterPick.name == "Edros") { GameObject.Find("EdrosPicked").GetComponent<Image>().enabled = true;
                GameObject.Find("TyrasPicked").GetComponent<Image>().enabled = false;
                GameObject.Find("AurianPicked").GetComponent<Image>().enabled = false;
            }
            if (StartGame.characterPick.name == "Tyras") { GameObject.Find("TyrasPicked").GetComponent<Image>().enabled = true;
                GameObject.Find("EdrosPicked").GetComponent<Image>().enabled = false;
                GameObject.Find("AurianPicked").GetComponent<Image>().enabled = false;
            }
            if (StartGame.characterPick.name == "Aurian") { GameObject.Find("AurianPicked").GetComponent<Image>().enabled = true;
                GameObject.Find("TyrasPicked").GetComponent<Image>().enabled = false;
                GameObject.Find("EdrosPicked").GetComponent<Image>().enabled = false;
            }
        }
        else
        {
            controller.localPlayer.CmdImReady(controller.localPlayer.isServer);
            if (StartGame.characterPick.name == "Edros")
            {
                GameObject.Find("EdrosPicked").GetComponent<Image>().enabled = true;
                GameObject.Find("TyrasPicked").GetComponent<Image>().enabled = false;
                GameObject.Find("AurianPicked").GetComponent<Image>().enabled = false;
            }
            if (StartGame.characterPick.name == "Tyras")
            {
                GameObject.Find("TyrasPicked").GetComponent<Image>().enabled = true;
                GameObject.Find("EdrosPicked").GetComponent<Image>().enabled = false;
                GameObject.Find("AurianPicked").GetComponent<Image>().enabled = false;
            }
            if (StartGame.characterPick.name == "Aurian")
            {
                GameObject.Find("AurianPicked").GetComponent<Image>().enabled = true;
                GameObject.Find("TyrasPicked").GetComponent<Image>().enabled = false;
                GameObject.Find("EdrosPicked").GetComponent<Image>().enabled = false;
            }
        }
    }
}