//Reveals the Text at the start.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEffect : MonoBehaviour
{
    public bool skip = false;
    bool skipping = false;
    public List<TextMeshProUGUI> leftText; List<string> leftString; 
    //Resets all the text variables.
    void Update() { if (!skipping && Input.anyKey) skipping = true; }
    void Start()
    {
        leftString = new List<string>();
        foreach (TextMeshProUGUI text in leftText) { leftString.Add(text.text); text.text = ""; }
        StartCoroutine(TypeWriterLeft());
    }
    //Loops through each text variable and slowly adds a character one by one to simulate a type writer effect.
    private IEnumerator TypeWriterLeft()
    {
        int i = 0; foreach (string text in leftString)
        {
            foreach (char character in text.ToCharArray())
            {
                if (skipping) { leftText[i].text = leftString[i]; break; }
                leftText[i].text += character;
                yield return new WaitForSeconds(0.01f);
            }
            i++;
        }
        skipping = true;
        skip = true;
    }
}