using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TextReader : MonoBehaviour
{
    public string path = "Assets/Resources/R Values.txt";
    public string text;

    public List<float> floatValues;

    private void Awake()
    {
        ReadStringToListOfFloat();
    }

    private void ReadStringToListOfFloat()
    {
        //Read the text from directly from the test.txt file
        TextAsset mytxtData = (TextAsset)Resources.Load(path);
        text = mytxtData.text;
        string[] values = text.Split('\n');
        foreach(string value in values)
        {
            floatValues.Add(float.Parse(value));
        }
    }
}
