﻿
using UnityEngine.UI;

public class StoryTagSelectSystem : EventSystemScript
{
    public StoryTagScript storyTagElement1;
    public StoryTagScript storyTagElement2;
    public StoryTagScript storyTagElement3;

    void Awake () {
        var storyTags = GameManagerScript.instance.assetManager.get3NewStoryTags();

        storyTagElement1.setStoryTag(storyTags[0]);
        storyTagElement2.setStoryTag(storyTags[1]);
        storyTagElement3.setStoryTag(storyTags[2]);
	}

    public void selectStoryTagElement(StoryTagScript storyTagElement)
    {
        GameManagerScript.instance.selectedStoryTag = storyTagElement.StoryTagObject;

        print("selected story tag:");
        print(storyTagElement.StoryTagObject.description);

        nextScene();
    }
}
