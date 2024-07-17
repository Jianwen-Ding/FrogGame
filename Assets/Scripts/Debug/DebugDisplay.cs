using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugDisplay : MonoBehaviour
{ 
    static DebugDisplay mainDisplay = null;
    // This class is meant to allow for easy continious showing of numbers/data

    [SerializeField]
    public Dictionary<string, string> displays = new Dictionary<string, string>();

    [SerializeField]
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        // Checks if there is only one debug displayer
        if(GameObject.FindObjectsOfType<DebugDisplay>().Length > 1)
        {
            Destroy(gameObject);
        }
        mainDisplay = this;
    }
    #region raycast checks
    #endregion
    #region data conversion
    public delegate string objectToString<T>(T foundObject);
    // Turns a array of objects into a string
    public static string arrayToString<T>(T[] array, objectToString<T> objectConverter)
    {
        string retString = "|";
        for(int i = 0; i < array.Length; i++)
        {
            retString += " " + objectConverter(array[i]) + " |";
        }
        return retString;
    }
    #endregion
    #region UI display
    // Creates a new display if key has already been repeated
    public static void updateDisplay(string key, string text)
    {
        if(mainDisplay != null)
        {
            if (mainDisplay.displays.ContainsKey(key))
            {
                mainDisplay.displays[key] = text;
            }
            else
            {
                mainDisplay.displays.Add(key, text);
            }
        }
        else
        {
//            print("display not initialized yet");
        }
    }

    // Deletes display
    public static void deleteDisplay(string key)
    {
        if(mainDisplay != null)
        {
            if (mainDisplay.displays.ContainsKey(key))
            {
                mainDisplay.displays.Remove(key);
            }
        }
        else
        {
            print("display not initialized yet");
        }
    }
    // Update is called once per frame
    void Update()
    {
        string retString = "";
        foreach(string key in displays.Keys)
        {
            retString += key + ": " + displays[key];
            retString += "<br>";
            retString += "<br>";
        }
        text.text = retString;
    }
    #endregion
}
