using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class textGenderBendScript : MonoBehaviour {
    
	void Start () {
        var text = GetComponent<Text>().text;   

        //print("GB changing text (isMale: " + GameManagerScript.instance.story.person.genderM + ")");
        //print(genderBend(text));

        GetComponent<Text>().text = genderBend(text);
	}

    public string genderBend(string text)
    {
        if (GameManagerScript.instance.story.person.genderM)
        {
            text = replaceText(text, "her", "him");
            text = replaceText(text, "she", "he");
            text = replaceText(text, "herself", "himself");
        }
        else
        {
            text = replaceText(text, "he", "she");
            text = replaceText(text, "him", "her");
            text = replaceText(text, "his", "her");
            text = replaceText(text, "himself", "herself");
        }

        return text;
    }

    private string replaceText(string text, string pattern, string replacement)
    {
        var isCapitalized = char.IsUpper(text, 0);

        // Only replace isolated words
        text = Regex.Replace(text.ToLower(), "\\b" + pattern + "\\b", replacement);

        if (isCapitalized)
        {
            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
        }
        return text;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
