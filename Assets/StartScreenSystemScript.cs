using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenSystemScript : EventSystemScript {

    public int playerCount = 0;
    public Text playerCountElement; // Defined in editor

    void Awake ()
    {
        //print("PCUI (awake): playercount: " + GameManagerScript.instance.playerCount);
    }

	void Start () {
        //playerCountElement = GameObject.Find("Player Count").GetComponent<Text>();
        //print("PCUI (start): playercount: " + GameManagerScript.instance.playerCount);
	}
	
	void Update () {
		if (GameManagerScript.instance.activePlayerCount != playerCount)
        {
            //print("Updating visible player count");

            //print("PCUI (update): playercount: " + GameManagerScript.instance.playerCount);

            playerCount = GameManagerScript.instance.activePlayerCount;
            playerCountElement.text = playerCount.ToString();
        }
	}
}
