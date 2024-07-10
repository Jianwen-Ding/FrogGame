using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

public class AnimalDistantDetector : MonoBehaviour
{
    // Detects all relavent objects
    #region vars
    [Header("Cache Variables")]
    [SerializeField]
    Collider col;
    AnimalPresent baseAnimal;

    [Header("Predator prey Parameters")]
    // To be imported by initialization
    // Tags of animals that this animal considers prey
    string[] preyTags;
    // Tags of animals that this animal considers predator
    string[] predatorTags;

    [Header("State Variables")]
    // Whether the object has been initialized or not
    bool initialized = false;

    // Animals within the field collider
    public List<GameObject> preyWithinField;
    public List<GameObject> predatorWithinField;
    #endregion
    // Start is called before the first frame update
    public void init(string[] preySet, string[] predSet, float size, AnimalPresent baseAnimalSet)
    {
        initialized = true;
        baseAnimal = baseAnimalSet;
        preyTags = preySet;
        predatorTags = predSet;
        transform.localScale = new Vector3(size, size, size);
        col = gameObject.GetComponent<Collider>();
    }

    // Adds prey to detected objects
    private void addPrey(GameObject prey)
    {
        preyWithinField.Add(prey);
        DebugDisplay.updateDisplay(prey.name, "prey");
    }

    // Adds predator to detected objects
    private void addPredator(GameObject predator)
    {
        predatorWithinField.Add(predator);
        DebugDisplay.updateDisplay(predator.name, "predator");
    }

    // Removes prey from detected objects
    private void removePrey(int index)
    {
        if (baseAnimal.preyAwareOf.ContainsKey(preyWithinField[index]))
        {
            baseAnimal.preyAwareOf.Remove(preyWithinField[index]);
        }
        DebugDisplay.updateDisplay("deleted", "deleted prey object");
        preyWithinField.RemoveAt(index);
    }

    // Removes prey from detected objects
    private void removePredator(int index)
    {
        if (baseAnimal.predatorsAwareOf.ContainsKey(predatorWithinField[index]))
        {
            baseAnimal.predatorsAwareOf.Remove(predatorWithinField[index]);
        }
        DebugDisplay.updateDisplay("deleted", "deleted predator object");
        predatorWithinField.RemoveAt(index);
    }

    // Checks entering colliders if they apply as prey or predator
    // Then adding them if they do
    private void OnTriggerEnter(Collider other)
    {
        if (initialized)
        {
            GameObject collidedWith = other.gameObject;
            if (customMathf.contains(preyTags, collidedWith.tag) && !preyWithinField.Contains(collidedWith))
            {
                addPrey(collidedWith);
            }
            if (customMathf.contains(predatorTags, collidedWith.tag) && !predatorWithinField.Contains(collidedWith))
            {
                addPredator(collidedWith);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (initialized)
        {
            GameObject collidedWith = other.gameObject;
            if (customMathf.contains(preyTags, collidedWith.tag) && !preyWithinField.Contains(collidedWith))
            {
                addPrey(collidedWith);
            }
            if (customMathf.contains(predatorTags, collidedWith.tag) && !predatorWithinField.Contains(collidedWith))
            {
                addPredator(collidedWith);
            }
        }
    }

    // Checks whether object does not exist anymore
    private void OnTriggerExit(Collider other)
    {
        if (initialized)
        {
            GameObject collidedWith = other.gameObject;
            if (preyWithinField.Contains(collidedWith))
            {
                removePrey(preyWithinField.FindIndex((ob) => ob ==collidedWith));
            }
            if (predatorWithinField.Contains(collidedWith))
            {
                removePredator(predatorWithinField.FindIndex((ob) => ob == collidedWith));
            }
        }
    }

    public void Update()
    {
        if (initialized)
        {
            for (int i = 0; i < predatorWithinField.Count; i++)
            {
                if (predatorWithinField[i] == null)
                {
                    removePredator(i);
                }
            }
            for (int i = 0; i < preyWithinField.Count; i++)
            {
                if (preyWithinField[i] == null)
                {
                    removePrey(i);
                }
            }
        }
    }
}
