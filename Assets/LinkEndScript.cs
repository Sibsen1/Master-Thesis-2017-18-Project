using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinkEndScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private StoryTagLinkSystem eventSystem;
    private RectTransform RT;

    private bool isDragging;
    private bool attached;

    public Vector3 startPosition;

    void Start()
    {
        eventSystem = FindObjectOfType<StoryTagLinkSystem>();
        RT = GetComponent<RectTransform>();

        startPosition = transform.position;
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = Input.mousePosition;
        }
        else if (!attached)
        {
            transform.position = startPosition + new Vector3(9 * Mathf.Sin(3 * Time.time), 0.0f, 0.0f); // Animated swinging
        }
    }

    public void attach(LinkAnchorScript anchor)
    {
        transform.position = anchor.transform.position;
        transform.parent = anchor.transform;
        attached = true;
        isDragging = false;

        try
        {
            transform.GetChild(0).GetComponent<Image>().enabled = false;
        } catch (NullReferenceException)
        {

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        transform.GetChild(0).GetComponent<Image>().enabled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;
        var storyV = GameObject.Find("Story Scroll View").GetComponent<RectTransform>();

        //Rect storyViewRect = new Rect(storyV.offsetMax.x, storyV.offsetMax.y,
         //   storyV.rect.width, storyV.rect.height);

        // Check if the linkEnd is inside the storyView
        // (for some reason, .localPosition is top-right and the only one that gives a relatable position for the comparison)
        if (storyV.localPosition.x >= RT.localPosition.x &&
            storyV.localPosition.x - storyV.rect.width <= RT.localPosition.x &&
            storyV.localPosition.y >= RT.localPosition.y &&
            storyV.localPosition.y - storyV.rect.height <= RT.localPosition.y)
        {
            eventSystem.attemptLinkAtPos(transform.position);
        }
    }
}
