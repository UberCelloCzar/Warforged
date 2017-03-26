﻿using UnityEngine;
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

		const string joeIP = "174.44.156.42";
        const string benIP = "50.30.232.143";
        const string jeremyIP = "192.168.2.12";
        const string steveIP = "129.21.141.231";
        const string trevorIP = "129.21.104.188";

        switch (playerName)
        {
            case "joe":
                ipAddressfield.text = joeIP;
                manager.networkAddress = joeIP;
                break;
            case "ben":
                ipAddressfield.text = benIP;
                manager.networkAddress = benIP;
                break;
            case "jeremy":
                ipAddressfield.text = jeremyIP;
                manager.networkAddress = jeremyIP;
                break;
            case "steve":
                ipAddressfield.text = steveIP;
                manager.networkAddress = steveIP;
                break;
            case "trevor":
                ipAddressfield.text = trevorIP;
                manager.networkAddress = trevorIP;
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
        }
        else
        {
            controller.localPlayer.CmdImReady(controller.localPlayer.isServer);
        }
    }
}