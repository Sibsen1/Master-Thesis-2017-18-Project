using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkScript : MonoBehaviour {

    public LineRenderer lineR;

    [Tooltip("Set active when both transform targets are set to make the link visible.")]
    public bool active;

    public Transform startTrans;
    public Transform endTrans;

    /*public RectTransform parentCanvasRT;
    public Canvas parentCanvas;
    public Camera parentCanvasCam;*/

    [Tooltip("Adjustment for coordinates if the link is not in the canvas.")]
    public float worldToScreenRatio = 31.25f;    // World size is 16:10,  Screen/Canvas is 500:312.22 ; 500/16 = 31.25
                                                 // TODO: Figure out how to get these numbers from the system

    void Start () {

        /*var par = transform.parent;
		while (par != null)
        {
            if (par.GetComponent<Canvas>() != null)
            {
                parentCanvasRT = par.GetComponent<RectTransform>(); ;
                parentCanvas = par.GetComponent<Canvas>();
                parentCanvasCam = Camera.main;
                print("Camera:" + parentCanvasCam);
                break;
            }
            par = par.parent;
        }*/
	}

	void Update () {
        if (active)
        {
            if (startTrans == null || endTrans == null)
            {
                print("LINK TARGET SUDDENLY BECAME NULL");
                active = false;
            }
            else
            {
                Vector3 startPos = Camera.main.ScreenToWorldPoint(startTrans.position);
                Vector3 endPos = Camera.main.ScreenToWorldPoint(endTrans.position);
                startPos.z = 0;
                endPos.z = 0;

                lineR.SetPosition(0, startPos);
                lineR.SetPosition(1, endPos);
                /*
                if (parentCanvasRT != null)
                {
                    Vector3 startPos;
                    Vector3 endPos;

                    RectTransformUtility.ScreenPointToWorldPointInRectangle(parentCanvasRT, startTrans.position,
                        parentCanvas.worldCamera, out startPos);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(parentCanvasRT, endTrans.position,
                        parentCanvasRT.GetComponent<Canvas>().worldCamera, out endPos);
                    
                    lineR.SetPosition(0, startPos);
                    lineR.SetPosition(1, endPos);

                    lineR.SetPosition(0, parentCanvasCam.ScreenToWorldPoint(startTrans.position));
                    lineR.SetPosition(1, parentCanvasCam.ScreenToWorldPoint(endTrans.position));

                    //lineR.SetPosition(0, transform.parent.InverseTransformPoint(startTrans.position) / worldToScreenRatio);
                    //lineR.SetPosition(1, transform.parent.InverseTransformPoint(endTrans.position) / worldToScreenRatio);

                    //lineR.SetPosition(0, transform.parent.InverseTransformPoint(startTrans.position) / worldToScreenRatio);
                    //lineR.SetPosition(1, transform.parent.InverseTransformPoint(endTrans.position) / worldToScreenRatio);
                }
                else
                {
                    lineR.SetPosition(0, transform.parent.InverseTransformPoint(startTrans.position) / worldToScreenRatio);
                    lineR.SetPosition(1, transform.parent.InverseTransformPoint(endTrans.position) / worldToScreenRatio);
                    //lineR.SetPosition(0, startTrans.position / worldToScreenRatio);
                    //lineR.SetPosition(1, endTrans.position / worldToScreenRatio);
                }*/
            }
        }
	}

    public bool setLink(Transform startTrans, Transform endTrans, bool useAnchors = false)
    {
        // Looks for linkAnchors if useAnchors is true; returns false if no anchors found
        if (startTrans == null || endTrans == null)
        {
            throw new NullReferenceException();
        }
        if (useAnchors)
        {
            var startAnchor = startTrans.GetComponentInChildren<LinkAnchorScript>();
            var endAnchor = endTrans.GetComponentInChildren<LinkAnchorScript>();

            if (startAnchor == null || endAnchor == null)
                return false;
            startTrans = startAnchor.transform;
            endTrans = endAnchor.transform;
        }

        active = true;
        this.startTrans = startTrans;
        this.endTrans = endTrans;

        print("Made link: " + startTrans + ", " + endTrans+ "; Positions: " + startTrans.position + ", " + endTrans.position);
        return true;
    }
}
