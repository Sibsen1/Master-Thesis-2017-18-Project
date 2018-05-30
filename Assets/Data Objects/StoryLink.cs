using UnityEngine;
using System.Collections;

public class StoryLink {

    public Trait traitLink;
    public StoryTag storyTagLink;

    public StoryTag endStoryTag;

    public StoryLink()
    {

    }

    public StoryLink(StoryTag parent, Trait link)
    {
        endStoryTag = parent;
        traitLink = link;
    }

    public StoryLink(StoryTag parent, StoryTag link)
    {
        endStoryTag = parent;
        storyTagLink = link;
    }
}

public struct StoryLinkStruct // Used for communicating across the net
{
    public int endStoryTagID;
    public int linkedElementID; // May be a Trait or another StoryTag
    public bool isLinkedToTrait; // Ottherwise linked to storytag
}
