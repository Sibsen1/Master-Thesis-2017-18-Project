using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class StoryTagLinkSystem : EventSystemScript {

    public StoryViewScript storyView;
    public StoryTagScript selectedStoryTag;
    public Button nextButton;

    public LinkScript linePrefab;
    public LinkAnchorScript linkAnchorPrefab;

    public LinkEndScript linkEnd;
    private LinkScript linkLine;

    StoryLink newLink;
    
	void Start ()
    {
        nextButton.interactable = false;

        storyView.attachTraitClickListener(SelectLinkToTrait);
        storyView.attachStoryTagClickListener(SelectLinkToStoryTag);

        linkEnd = FindObjectOfType<LinkEndScript>();
        
        newLink = new StoryLink();
        newLink.endStoryTag = gameManager.selectedStoryTag;

        foreach (var sTag in FindObjectsOfType<StoryTagScript>())
        {
            if (sTag.GetComponentInChildren<LinkAnchorScript>() == null)
            {
                var newAnchor = Instantiate(linkAnchorPrefab);
                newAnchor.GetComponent<RectTransform>().SetParent(sTag.transform, false);
                //newAnchor.transform.parent = sTag.transform;

                //newAnchor.transform.localPosition -= new Vector3(0, 10, 0);
            }
        }
    }

    void SelectLinkToTrait(TraitScript trait)
    {
        newLink.storyTagLink = null;
        newLink.traitLink = trait.TraitObject;

        nextButton.interactable = true; 
        
        print("Selecting trait");
        print(trait.transform.position);

        putLine(trait.transform);

        //storyView.makeCurve(trait.transform.position, selectedStoryTag.transform.position);

    }

    void SelectLinkToStoryTag(StoryTagScript sTag)
    {
        newLink.storyTagLink = sTag.StoryTagObject;
        newLink.traitLink = null;

        nextButton.interactable = true;

        putLine(sTag.transform);

        //storyView.makeCurve(sTag.transform.position, selectedStoryTag.transform.position);
    }

    private void putLine(Transform endPos)
    {
        var targetAnchor = endPos.GetComponentInChildren<LinkAnchorScript>();

        if (targetAnchor != null)
        {
            linkEnd.attach(targetAnchor);
        }
        else
        {
            print("STLinkSystem: Failed creating link - Target Link Anchor not found");
        }
    }

    internal bool attemptLinkAtPos(Vector3 position) // Called from the Link End being dropped by the player
    {
        foreach (var anchor in FindObjectsOfType<LinkAnchorScript>())
        {
            if (anchor.isRecipient && Vector3.Distance(anchor.transform.position, position) <= 40)
            {
                var traitC = anchor.transform.parent.GetComponent<TraitScript>();
                var sTagC = anchor.transform.parent.GetComponent<StoryTagScript>();

                if (traitC != null)
                {
                    SelectLinkToTrait(traitC);
                    return true;
                } else if (sTagC != null)
                {
                    SelectLinkToStoryTag(sTagC);
                    return true;
                }
            }
        }
        return false;
    }

    public new void nextScene()
    {
        print("Adding sTag");
        gameManager.addStoryTag(gameManager.selectedStoryTag, newLink);
        base.nextScene();
    }

    void Update () {
	
	}
}
