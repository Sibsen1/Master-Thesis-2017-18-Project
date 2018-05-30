using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerScript : NetworkBehaviour
{
    GameManagerScript gameManager;
    [SyncVar(hook = "OnNewID")]
    public int ID;

    public bool isActivePlayer = true; // Set to false if this player should not be part of the turn rotations

    public void Awake()
    {
        gameManager = GameManagerScript.instance;
        isActivePlayer = gameManager.isActivePlayer;
    }

    public override void OnStartLocalPlayer()
    {
        print("Starting local player");

        base.OnStartLocalPlayer();
        gameManager.localPlayer = this;

        //print(GameManagerScript.instance.playerCount);

        //GameManagerScript.instance.playerCount = GameManagerScript.instance.networkManager.numPlayers;
        //GameManagerScript.instance.localPlayer = GameManagerScript.instance.networkManager.numPlayers - 1;

        //print(GameManagerScript.instance.networkManager.numPlayers);
        //print(GameManagerScript.instance.networkManager.client.connection.playerControllers.Count);
    }

    public void nextTurn()
    {
        print("Player: Next turn");
        CmdNextTurn();
    }

    [Command]
    public void CmdNextTurn()
    {
        print("CmdNextTurn");

        gameManager.nextTurn();

        // Moved to gameManager - now this command is just to make sure the gameManager runs its method on the server

        /*gameManager.turnsPlayed += 1;
        gameManager.currentPlayer = gameManager.currentPlayer >= gameManager.playerCount - 1 ? 0 : gameManager.currentPlayer + 1;
        // Go through the players round-robin style

        if (gameManager.turnsPlayed >= 5)
        {
            gameManager.RpcEndGame();
            return;
        }*/
    }

    [Command]
    public void CmdSetPerson(int personID)
    {
        print("CmdSetPerson");
        gameManager.setPerson(personID);
    }

    [Command]
    public void CmdAddStoryTag(int sTagID, StoryLinkStruct sLinkStruct)
    {
        print("CmdAddStoryTag");
        //gameManager.addStoryTag(sTagID, sLinkStruct);

        gameManager.RpcAddStoryTag(sTagID, sLinkStruct, gameManager.turnsPlayed);
    }

    [Command]
    public void CmdEndGame()
    {
        print("CmdEndGame");
        gameManager.RpcEndGame();
    }

    public void OnNewID(int newID)
    {
        print("Player: Registered new ID: " + newID);
        ID = newID;
    }
}