using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class TraitScript : MonoBehaviour
{
    [Tooltip("Whether to default to the Person present in the current story object (stored by GameManager).")]
    public bool useStoryPerson = true;

    [Tooltip("Which Trait index of the Person should this display?")]
    public int traitIndex;

    public Trait TraitObject;
    
    void Awake () {
        if (useStoryPerson)
        {
            setTrait(GameManagerScript.instance.story.person.traits[traitIndex]);
        }
	}

    void Start()
    {

    }

    private void setTrait(Trait trait)
    {
        TraitObject = trait;
        GetComponent<Text>().text = trait.description;
    }
    
    void Update () {
	
	}
}
