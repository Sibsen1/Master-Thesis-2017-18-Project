using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharSelectSystem : EventSystemScript {

    public PersonElementScript personElement1;  // These should be linked in the editor
    public PersonElementScript personElement2;
    public PersonElementScript personElement3;

    private List<Person> personsList;

    void Start () {
        personsList = GameManagerScript.instance.assetManager.getPersons(3);

        personElement1.GetComponent<PersonElementScript>().setPerson(personsList[0]);
        personElement2.GetComponent<PersonElementScript>().setPerson(personsList[1]);
        personElement3.GetComponent<PersonElementScript>().setPerson(personsList[2]);
    }

    public void selectCharacter(PersonElementScript personElement)
    {
        print("New character selected");

        if (personElement == personElement1)
        {
            GameManagerScript.instance.setPerson(personsList[0]);
        }
        else if (personElement == personElement2)
        {
            GameManagerScript.instance.setPerson(personsList[1]);
        }
        else if (personElement == personElement3)
        {
            GameManagerScript.instance.setPerson(personsList[2]);
        }

        nextScene();
    }
}
