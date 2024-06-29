using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDistantDetector : MonoBehaviour
{
    // Detects all relavent objects
    #region vars
    [Header("Cache Variables")]
    [SerializeField]
    Collider col;

    [Header("Predator prey Parameters")]
    // To be imported by initialization
    // Tags of animals that this animal considers prey
    string[] preyTags;
    // Tags of animals that this animal considers predator
    string[] predatorTags;

    [Header("State Variables")]
    public List<GameObject> preyWithinField;
    public List<GameObject> predatorWithinField;
    #endregion
    // Start is called before the first frame update
    public void init(string[] preySet, string[] predSet, float size)
    {
        preyTags = preySet;
        predatorTags = predSet;
        transform.localScale = new Vector3(size, size, size);
        col = gameObject.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidedWith = other.gameObject;
        if (customMathf.contains(preyTags, collidedWith.tag) && !preyWithinField.Contains(collidedWith))
        {
            preyWithinField.Add(collidedWith);
            DebugDisplay.updateDisplay(collidedWith.name, "prey");
        }
        if (customMathf.contains(predatorTags, collidedWith.tag) && !predatorWithinField.Contains(collidedWith))
        {
            predatorWithinField.Add(collidedWith);
            DebugDisplay.updateDisplay(collidedWith.name, "predator");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collidedWith = other.gameObject;
        if (preyWithinField.Contains(collidedWith))
        {
            preyWithinField.Remove(collidedWith);
            DebugDisplay.deleteDisplay(collidedWith.name);
        }
        if (predatorWithinField.Contains(collidedWith))
        {
            predatorWithinField.Remove(collidedWith);
            DebugDisplay.deleteDisplay(collidedWith.name);
        }
    }
}
