using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PersonElementScript : MonoBehaviour
{
    [Tooltip("Whether to default to the Person present in the current story object (stored by GameManager).")]
    public bool useStoryPerson;

    public Person personObject;
    
    void Start () {
        if (personObject != null)
        {
            setPerson(personObject);

        } else if (useStoryPerson)
        {
            setPerson(GameManagerScript.instance.story.person);
        }
	}

    public void setPerson(Person personObject)
    {
        this.personObject = personObject;

        GetComponentInChildren<Text>().text = personObject.personName;
        transform.Find("Person Picture").gameObject.GetComponent<Image>().sprite = personObject.portrait;
    }

    // Update is called once per frame
    void Update () {
	
	}


}
