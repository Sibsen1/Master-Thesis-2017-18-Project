using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class StoryTagScript : MonoBehaviour
{
    [Tooltip("Whether to default to the currently selected StoryTag in GameManager.")]
    public bool useSelectedStoryTag;

    public StoryTag StoryTagObject;
    
    public void Awake () {
        if (useSelectedStoryTag)
        {
            setStoryTag(GameManagerScript.instance.selectedStoryTag);
        }
	}

    public void setStoryTag(StoryTag storytag)
    {
        StoryTagObject = storytag;
        GetComponent<Text>().text = storytag.description;
    }
    
    void Update () {
	
	}
}
