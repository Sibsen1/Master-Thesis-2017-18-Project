using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class NetworkManagerScript : NetworkManager {

    NetworkMatch networkMatcher;
    public bool singlePlayerDebug;

    public void Awake()
    {
        print("NetworkManager awake");

        networkMatcher = gameObject.AddComponent<NetworkMatch>();
        networkMatcher.ListMatches(0, 1, "", true, 0, 0, ConnectToOrCreateMatch);

        if (singlePlayerDebug)
        {
            try
            {
                GetComponent<NetworkManagerHUD>().showGUI = true;
            }
            catch (NullReferenceException)
            { }
        }
    }
    
    public void ConnectToOrCreateMatch(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (singlePlayerDebug && matches != null && matches.Count > 0)
            {
                networkMatcher.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);
            } else
            {
                print("Matches (" + matches.Count + "): " + matches);
                networkMatcher.CreateMatch("defaultRoomTest", 20, true, "", "mm.unet.unity3d.com", "", 0, 0, OnMatchCreate);
            }
            
        }
        else if (!success)
        {
            print("Failed listing matches: " + extendedInfo);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        GameManagerScript.instance.playerCount = numPlayers;
        GameManagerScript.instance.registerPlayers();

        print("NM: Added player ID " + conn.connectionId + 
            "(addr: "+ conn.address +", total players: "+ numPlayers+")");
    }
}
