using UnityEngine;
using System.Collections;

public class StoryTag {
    public string description;
    public bool isSolution = false;

    public StoryTag(string description)
    {
        this.description = description;
    }

    public StoryTag(string description, bool isSolution)
    {
        this.description = description;
        this.isSolution = isSolution;
    }
}
