using UnityEngine;
using System.Collections.Generic;
using System;

public class AssetManagerScript : MonoBehaviour {

    public static AssetManagerScript instance;

    public List<Person> personList;
    public List<StoryTag> storyTagList;
    public List<StoryTag> storyTagSolutionList;
    public List<Trait> traitList;

    private Dictionary<string, Sprite> portraitSprites;
    private List<StoryTag> freshStoryTags;
    public bool shuffleSTagList; // Only for use in Single Player

    void Awake()
    {
        if (instance != null && instance != this)
        {
            print("Destroying duplicate AssetManager");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start () {
        personList = new List<Person>();
        storyTagList = new List<StoryTag>();
        storyTagSolutionList = new List<StoryTag>();
        traitList = new List<Trait>();

        portraitSprites = new Dictionary<string, Sprite>();


        createAssets();
	}

    private void createAssets()
    {
        foreach (var spr in Resources.LoadAll("Portraits", typeof(Sprite)))
        {
            portraitSprites.Add(spr.name, spr as Sprite);
        }
        traitList.Add(new Trait("Student"));
        traitList.Add(new Trait("Too nice for her own good"));
        traitList.Add(new Trait("Plays a sport regularly - which?"));

        personList.Add(new Person("Laura", false, portraitSprites["Laura"],
            traitList[0], traitList[1], traitList[2]));

        traitList.Add(new Trait("Truckdriver"));
        traitList.Add(new Trait("Barely makes enough for rent"));
        traitList.Add(new Trait("Has a wife with special needs - what kind?"));

        personList.Add(new Person("Toby", true, portraitSprites["Toby"],
            traitList[3], traitList[4], traitList[5]));

        traitList.Add(new Trait("Teacher"));
        traitList.Add(new Trait("Has a daughter named Eva"));
        traitList.Add(new Trait("Follows a religion - which?"));

        personList.Add(new Person("Dean", true, portraitSprites["Dean"],
            traitList[6], traitList[7], traitList[8]));

        storyTagList.Add(new StoryTag("He gets hurt badly"));
        storyTagList.Add(new StoryTag("He travels far away"));
        storyTagList.Add(new StoryTag("His home is destroyed"));

        storyTagList.Add(new StoryTag("He gets an untrustworthy friend"));
        storyTagList.Add(new StoryTag("He gets accused of a crime"));
        storyTagList.Add(new StoryTag("He tries to change himself"));

        storyTagList.Add(new StoryTag("He gathers the community for a cause"));
        storyTagList.Add(new StoryTag("He gets arrested and taken to court"));
        storyTagList.Add(new StoryTag("He gets the wrong facts about something"));

        storyTagList.Add(new StoryTag("He is chased by a group"));
        storyTagList.Add(new StoryTag("He becomes furious because of something"));
        storyTagList.Add(new StoryTag("Someone in his family becomes seriously ill"));

        storyTagList.Add(new StoryTag("He commits a crime"));
        storyTagList.Add(new StoryTag("He hurts someone by accident"));
        storyTagList.Add(new StoryTag("He spends all his money on something"));
        
        // TODO: Shuffle the same way across all clients
        if (shuffleSTagList)
        {
            for (int i = 0; i < storyTagList.Count; i++)
            {
                var temp = storyTagList[i];
                var randomIndex = UnityEngine.Random.Range(i, storyTagList.Count);
                storyTagList[i] = storyTagList[randomIndex];
                storyTagList[randomIndex] = temp;
            }
        }
        freshStoryTags = new List<StoryTag>(storyTagList);
        freshStoryTags.Reverse();

        storyTagSolutionList.Add(new StoryTag("Someone famous comes to help", true));
        storyTagSolutionList.Add(new StoryTag("A change in our social structures", true));
        storyTagSolutionList.Add(new StoryTag("The power of self-belief!", true));
        storyTagSolutionList.Add(new StoryTag("He gets a superpower", true));
        storyTagSolutionList.Add(new StoryTag("Something you can do with money", true));
        storyTagSolutionList.Add(new StoryTag("If people understood something better", true));
        storyTagSolutionList.Add(new StoryTag("He becomes a leader and does something", true));
        storyTagSolutionList.Add(new StoryTag("If we were there and helped ...", true));
        storyTagSolutionList.Add(new StoryTag("If society treated a group better", true));

        // Shuffle storyTagSolutionList:
        for (int i = 0; i < storyTagSolutionList.Count; i++)
            {
                var temp = storyTagSolutionList[i];
                var randomIndex = UnityEngine.Random.Range(i, storyTagSolutionList.Count);
                storyTagSolutionList[i] = storyTagSolutionList[randomIndex];
                storyTagSolutionList[randomIndex] = temp;
            }
    }

    public int getID(Person person)
    {
        return personList.IndexOf(person);
    }

    public int getID(Trait trait)
    {
        return traitList.IndexOf(trait);
    }

    public int getID(StoryTag sTag)
    {
        return storyTagList.IndexOf(sTag);
    }

    public List<Person> getPersons(int personID)
    {
        List<Person> output = new List<Person>();

        output.AddRange(personList);

        return output;
    }


    public List<StoryTag> get3NewStoryTagSolutions()
    {
        return storyTagSolutionList.GetRange(0, 3);
    }

    public List<StoryTag> get3NewStoryTags() // Always tries to return 3 new storytags
    {
        try
        {
            return freshStoryTags.GetRange((GameManagerScript.instance.turnsPlayed - 1) * 3, 3);
        } catch (ArgumentOutOfRangeException)
        {
            return freshStoryTags.GetRange(
                (GameManagerScript.instance.turnsPlayed * 3) % freshStoryTags.Count, 3);
        }
        
    }
}
