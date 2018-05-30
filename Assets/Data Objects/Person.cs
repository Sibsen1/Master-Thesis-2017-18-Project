using System.Collections.Generic;
using UnityEngine;

public class Person
{
    public string personName;
    public bool genderM;

    public List<Trait> traits;
    public Sprite portrait;

    public Person(string personName, bool genderM, Sprite portrait, params Trait[] traits)
    {
        this.personName = personName;
        this.genderM = genderM;
        this.portrait = portrait;

        this.traits = new List<Trait>();
        this.traits.AddRange(traits);
    }
}
