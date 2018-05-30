using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Story
{
    public Person person;

    public List<StoryTag> storytags;
    public List<StoryLink> storylinks; // Corresponds 1-to-1 with storytags

    private List<Action<StoryTag, StoryLink>> newStoryTagListeners;

    public Story()
    {
        storytags = new List<StoryTag>();
        storylinks = new List<StoryLink>();
        newStoryTagListeners = new List<Action<StoryTag, StoryLink>>();
    }

    public void setPerson (Person person)
    {
        this.person = person;
    }

    public void addStoryTag (StoryTag sTag, StoryLink sLink)
    {
        storytags.Add(sTag);
        storylinks.Add(sLink);

        foreach (var onNewSTag in newStoryTagListeners)
        {
            onNewSTag(sTag, sLink);
        }
    }

    public void attachNewStoryTagListener(Action<StoryTag, StoryLink> onNewSTag)
    {
        newStoryTagListeners.Add(onNewSTag);
    }
}