using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

public class GameManagerScript : NetworkBehaviour {

    public static GameManagerScript instance = null;   // Static instance of GameManager which allows it to be accessed by any other script.

    public AssetManagerScript assetManager; // Should be given in editor
    public NetworkManager networkManager; // Should be given in editor

    public Story story;
    
    private int currentScene;
    [SyncVar]
    public int playerCount; // Updated by the NetworkManager
    [SyncVar]
    public int activePlayerCount; // TODO: Less redundant way of communicating this between clients
    [SyncVar(hook = "OnNewTurn")]
    public int turnsPlayed; // Incremented every time the character or a storytag is added by a player
    public int currentPlayer; // The player whose turn it is 
    public NetworkPlayerScript localPlayer; // Get connectionID from localPlayer.ID
    private int lastStoryTagTurn;

    public StoryTag selectedStoryTag;
    public Camera mainCamera;
    private List<NetworkPlayerScript> playerList;

    public bool isActivePlayer = true;
    public int turnsPerPlayer = 2;

    void Awake () {

        print("GameManager Awake");

        if (instance != null && instance != this)
        {
            print("Destroying duplicate GameManager");
            Destroy(gameObject);
            return;
        }
        story = new Story();
        playerList = new List<NetworkPlayerScript>();
        instance = this;
    }

    public override void OnStartServer()
    {
        print("Server Starting");
        base.OnStartServer();
        try
        {
            networkManager.GetComponent<NetworkManagerHUD>().showGUI = false;
        }
        catch (NullReferenceException)
        { }
    }

    public void registerPlayers()
    {
        if (!isServer)
            return;

        var foundPlayers = new List<NetworkPlayerScript>(FindObjectsOfType<NetworkPlayerScript>());
        
        foreach (var player in playerList)
        {
            if (!foundPlayers.Contains(player))
            {
                playerList.Remove(player);
                continue;
            }
        }

        activePlayerCount = 0;
        foreach (var player in foundPlayers)
        {
            if (!player.isActivePlayer)
                continue;

            activePlayerCount += 1;
            if (!playerList.Contains(player))
            {
                int newPlayerID = getNewPLayerID();
                player.ID = newPlayerID;
                playerList.Add(player);
                print("Registered player: " + newPlayerID);
            }
        }
    }

    private int getNewPLayerID()
    {
        var playerIDsInUse = new List<int>();
        foreach (var player in playerList)
        {
            playerIDsInUse.Add(player.ID);
        }

        int newPlayerID = 0;
        // Find the lowest playerID that isn't in use
        while (playerIDsInUse.Contains(newPlayerID))
        {
            newPlayerID += 1;
        }

        return newPlayerID;
    }

    public override void OnStartClient()
    {
        print("Client Starting");
        base.OnStartClient();

        if (isActivePlayer)
            LoadScene(SceneConstants.START);
        else
            LoadScene(SceneConstants.STORY_SCREEN);
    }
    
    public void nextTurn()
    {
        if (!isServer)
        {
            print("Rerouting next turn call through local player Command");
            localPlayer.CmdNextTurn();
            return;
        }
        turnsPlayed += 1;
        print("Next turn");
    }
    
    public void OnNewTurn(int newTurnsPlayed) // Updated on each client once turnsPlayed changes
    {
        turnsPlayed = newTurnsPlayed;

        // Go through the players round-robin style:
        currentPlayer = currentPlayer >= activePlayerCount - 1 ? 0 : currentPlayer + 1;

        if (!isActivePlayer)
        {
            LoadScene(SceneConstants.STORY_SCREEN, true); // Reload the view every new turn
            return;
        }
        
        if (turnsPlayed > activePlayerCount * turnsPerPlayer)
        {
            print("GameManager: Switching to Solution Select; turnsPlayed (" + turnsPlayed 
                + ") > activePlayerCount (" + activePlayerCount + ")");
            LoadScene(SceneConstants.SELECT_SOLUTION);
            return;
        }

        print("Starting new turn (scene: "+ currentScene + ", current player: " + currentPlayer
            + ", local player: "+ localPlayer.ID + ")");
        if (currentPlayer == localPlayer.ID)
        {
            LoadScene(SceneConstants.TURN_START);
        } else
        {
            LoadScene(SceneConstants.WAITING_FOR_PLAYERS);
        }
    }

    [ClientRpc]
    public void RpcEndGame()
    {
        if (!isActivePlayer)
            return;

        LoadScene(SceneConstants.GAME_END);
    }

    public void NextScene()
    {
        if (currentScene + 1 == SceneConstants.GAME_END)
        {
            localPlayer.CmdEndGame();
            return;
        }

        LoadScene(currentScene + 1);
    }

    public void previousScene()
    {
        LoadScene(currentScene - 1);
    }

    public void LoadScene(int newScene, bool reload=false)
    {
        // If reload is set to true, the method will load the scene even if it's already loaded
        if (newScene < 0 || currentScene == newScene && !reload)
        {
            return;
        }

        if (currentScene != SceneConstants.BASE
            && currentScene != SceneConstants.WAITING_FOR_PLAYERS
            && newScene < SceneConstants.SELECT_SOLUTION
            && currentPlayer != localPlayer.ID)
        {
            // TODO: Instead use game states to decide when a turn is in progress and when to start/end the game
            newScene = SceneConstants.WAITING_FOR_PLAYERS;
            print("Switching to waiting screen (curr: " + currentPlayer + ")");
        }

        int scenesUnloaded = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex != 0)
            {
                SceneManager.UnloadSceneAsync(scene);
                scenesUnloaded++;
            }
        }

        print("Switching to scene " + newScene + " from scene " + currentScene
            + " (current turn: " + turnsPlayed + ", unloaded " + scenesUnloaded + " scenes)");

        SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
        currentScene = newScene;
    }

    public void startGame() // Not currently in use
    {
        LoadScene(SceneConstants.SELECT_PERSON);
    }

    public void setPerson(Person person)
    {
        print("Converting person for transmission");
        setPerson(assetManager.getID(person));
    }

    public void setPerson(int personID)
    {
        if (!isServer)
        {
            print("Rerouting set person through player command");
            localPlayer.CmdSetPerson(personID);
            return;
        }
        print("Server: Setting person");

        RpcSetPerson(personID);

    }

    public void addStoryTag(StoryTag sTag, StoryLink sLink)
    {
        print("Converting storytag for transmission");
        var sTagID = assetManager.getID(sTag);

        var sLinkStruct = new StoryLinkStruct();
        sLinkStruct.endStoryTagID = sTagID;
        
        if (sLink.traitLink != null)
        {
            sLinkStruct.isLinkedToTrait = true;
            sLinkStruct.linkedElementID = assetManager.getID(sLink.traitLink);
        }
        else if (sLink.storyTagLink != null)
        {
            sLinkStruct.isLinkedToTrait = false;
            sLinkStruct.linkedElementID = assetManager.getID(sLink.storyTagLink);
        } else
        {
            return;
        }

        addStoryTag(sTagID, sLinkStruct);
    }

    public void addStoryTag(int sTagID, StoryLinkStruct sLinkStruct)
    {
        if (!isServer)
        {
            print("Rerouting adding story tag to local player command");
            localPlayer.CmdAddStoryTag(sTagID, sLinkStruct);
            return;
        }
        print("Adding storyTag");

        RpcAddStoryTag(sTagID, sLinkStruct, turnsPlayed);
    }

    [ClientRpc] // Called by the server to each client
    public void RpcSetPerson(int personID)
    {
        story.setPerson(AssetManagerScript.instance.personList[personID]);
    }

    [ClientRpc] // Called by the server to each client
    public void RpcAddStoryTag(int storyTagID, StoryLinkStruct sLink, int currentTurn)
    {
        if (lastStoryTagTurn == currentTurn)
            return;

        lastStoryTagTurn = currentTurn;
        var sTag = AssetManagerScript.instance.storyTagList[storyTagID];

        if (sLink.isLinkedToTrait)
        {
            print("RPC: Adding new storytag t");
            story.addStoryTag(sTag,
                new StoryLink(sTag, AssetManagerScript.instance.traitList[sLink.linkedElementID]));
        }
        else
        {
            print("RPC: Adding new storytag s");
            story.addStoryTag(sTag,
                new StoryLink(sTag, AssetManagerScript.instance.storyTagList[sLink.linkedElementID]));
        }
    }
}

static class SceneConstants
{
    public const int BASE = 0;
    public const int START = 1;
    public const int SELECT_PERSON = 2;
    public const int TURN_START = 5;
    public const int WAITING_FOR_PLAYERS = 9;
    public const int SELECT_SOLUTION = 10;
    public const int GAME_END = 13;
    public const int STORY_SCREEN = 14;
}