using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventSystemScript : MonoBehaviour {

    protected GameManagerScript gameManager = GameManagerScript.instance;
    
	void Start () {
        gameManager = GameManagerScript.instance;
	}
	
	void Update () {

    }

    public void nextScene()
    {
        gameManager.NextScene();
    }

    public void nextTurn()
    {
        gameManager.nextTurn();
    }

    public void previousScene()
    {
        gameManager.previousScene();
    }

    public void setPlayerCount(int players)
    {
        GameManagerScript.instance.playerCount = players;
    }

    public void setPlayerCount(Dropdown playerCountDropdown)
    {
        GameManagerScript.instance.playerCount = playerCountDropdown.value+1;
    }

    public void toggleHidden(MaskableGraphic UIElement)
    {
        UIElement.color = new Color(UIElement.color.r, UIElement.color.g, UIElement.color.b, UIElement.color.a == 0f ? 255f : 0);
    }
}
