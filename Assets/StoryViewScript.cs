using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class StoryViewScript : MonoBehaviour {

    readonly Vector3 defaultSpawnPos = new Vector3(370, -60);

    public Text storyTagPrefab; // Assigned in editor; the prefab which will be spawned to represent StoryTags
    public LinkScript linePrefab;

    Transform contentField;
    List<StoryTagScript> storyTagElements;
    List<TraitScript> traitElements;
    List<StoryLink> storyLinks;
    List<LinkScript> linkLines;

    List<Action<TraitScript>> traitClickListeners;
    List<Action<StoryTagScript>> storyTagClickListeners;

    void Awake ()
    {
        contentField = transform.Find("Viewport").Find("Content");
        storyTagElements = new List<StoryTagScript>();
        traitElements = new List<TraitScript>();
        storyLinks = new List<StoryLink>();
        linkLines = new List<LinkScript>();
        traitClickListeners = new List<Action<TraitScript>>();
        storyTagClickListeners = new List<Action<StoryTagScript>>();
        
        // Populate the traits, storytags and links from the Story:

        var traitsBox = contentField.Find("Traits Box");

        for (int i = 0; i < traitsBox.childCount; i++)
        {
            var traitElement = traitsBox.GetChild(i).GetComponent<TraitScript>();

            if (traitElement != null)
            {
                traitElements.Add(traitElement);

                foreach (var listener in traitClickListeners)
                {
                    traitElement.GetComponent<Button>().onClick.AddListener(() => listener(traitElement));
                }
            }
        }

        foreach (StoryTag sTag in GameManagerScript.instance.story.storytags)
        {
            addStoryTag(sTag);
        }
    }

    void Start()
    {
        foreach (StoryLink slink in GameManagerScript.instance.story.storylinks)
        {
            addStoryLink(slink);
        }

        GameManagerScript.instance.story.attachNewStoryTagListener(onNewStoryTagAdded);

        autofitContentField();
        GetComponentInChildren<ScrollRect>().horizontalNormalizedPosition = 1;

        if (GameManagerScript.instance.story.person == null)
        {
            Destroy(GameObject.Find("Person Element"));
            Destroy(GameObject.Find("Traits Box"));
        }
    }

    public void addStoryTag(StoryTag sTag) // Add a story tag element to the view
    {
        Text storyTagElement = Instantiate(storyTagPrefab, contentField) as Text;
        var storyTagScript = storyTagElement.GetComponent<StoryTagScript>();

        storyTagScript.setStoryTag(sTag);

        float elementWidth = storyTagElement.rectTransform.rect.width * 0.8f;

        storyTagElement.rectTransform.localPosition = new Vector3(
            defaultSpawnPos.x + elementWidth * storyTagElements.Count,
            defaultSpawnPos.y, 
            0);

        storyTagElement.rectTransform.localScale = new Vector3(0.8f, 0.8f);
        
        storyTagElements.Add(storyTagElement.GetComponent<StoryTagScript>());  // TODO: Make the prefab require a StoryTagScrit

        foreach (var listener in storyTagClickListeners)
        {
            storyTagElement.GetComponent<Button>().onClick.AddListener(() => listener(storyTagScript));
        }

        autofitContentField();
    }

    private void autofitContentField()
    {
        // Find the furthest right elemment (with RectTransform) and see how 
        // far we need to stretch the content field to accommodate it

        var contentRectT = contentField.GetComponent<RectTransform>();
        float highestX = 0;

        for (int i = 0; i < contentField.childCount; i++)
        {
            var rectT = contentField.GetChild(i).GetComponent<RectTransform>();

            if (rectT != null && rectT.offsetMax.x > highestX)
            {
                highestX = rectT.offsetMax.x;
            }
        }
        if (true)// highestX > contentRectT.sizeDelta.x)
        {
            print("StoryView: Autofitting, changing size from " + contentRectT.sizeDelta.x + " to " + highestX);
            var contentSize = contentRectT.sizeDelta;
            contentSize.x = highestX;

            contentRectT.sizeDelta = contentSize;
        }

    }

    public void addStoryLink(StoryLink sLink)
    {
        storyLinks.Add(sLink);

        var endStoryTag = sLink.endStoryTag;

        Transform startPos = null;
        Transform endPos = null;

        foreach (StoryTagScript sTag in storyTagElements)
        {
            print("StoryView: Found end stag: " + sTag.StoryTagObject.description
                + ", match: " + endStoryTag.description);
            if (sTag.StoryTagObject.description == endStoryTag.description)
            {
                endPos = sTag.transform;
                break;
            }
        }

        if (sLink.storyTagLink != null)
        {
            var linkedStoryTag = sLink.storyTagLink;
            foreach (StoryTagScript sTag in storyTagElements)
            {
                print("StoryView: Found sTag: " + sTag.StoryTagObject.description 
                    + ", match: " + linkedStoryTag.description);
                if (sTag.StoryTagObject.description == linkedStoryTag.description)
                {
                    startPos = sTag.transform;
                    break;
                }
            }

            if (startPos == null)
            {
                print("unable to find storytag in story");
            }
        } else
        {
            var linkedtrait = sLink.traitLink;
            foreach (TraitScript traitS in traitElements)
            {
                if (traitS.TraitObject.description == linkedtrait.description)
                {
                    startPos = traitS.GetComponent<Text>().transform;
                    break;
                }
            }

            if (startPos == null)
            {
                print("unable to find trait in story");
            }
        }
        

        makeLink(startPos, endPos);
    }

    public void onNewStoryTagAdded(StoryTag sTag, StoryLink sLink)
    {
        foreach (var sTagE in storyTagElements)
        {
            if (sTagE.StoryTagObject == sTag)
                return;
        }
        addStoryTag(sTag);
        addStoryLink(sLink);
    }

    public void attachTraitClickListener(Action<TraitScript> onClick)
    {
        traitClickListeners.Add(onClick);
        foreach (TraitScript trait in traitElements)
        {
            trait.GetComponent<Button>().onClick.AddListener(() => onClick(trait));
        }
    }

    public void attachStoryTagClickListener(Action<StoryTagScript> onClick)
    {
        storyTagClickListeners.Add(onClick);
        foreach (StoryTagScript sTag in storyTagElements)
        {
            sTag.GetComponent<Button>().onClick.AddListener(() => onClick(sTag.GetComponent<StoryTagScript>()));
        }
    }

    public void makeLink(Transform startpos, Transform endPos)
    {
        var linkLine = Instantiate(linePrefab, transform.parent, true);

        if (!linkLine.setLink(startpos, endPos, true))
        {
            // First try making link between anchors, then default to the objects themselves:
            linkLine.setLink(startpos, endPos);
        }

        linkLines.Add(linkLine);
    }
}
