using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using UnityEditorInternal.VR;
using UnityEngine;

public class AnimalDistantDetector : MonoBehaviour
{
    // Detects all relavent objects
    #region vars
    [Header("Cache Variables")]
    AnimalPresent baseAnimal;

    [Header("Predator prey Parameters")]
    [SerializeField]
    float distanceUntilDetect;
    // To be imported by initialization
    // Tags of animals that this animal considers prey
    string[] preyTags;
    // Tags of animals that this animal considers predator
    string[] predatorTags;

    [SerializeField]
    float detectTime;

    [Header("State Variables")]
    // Whether the object has been initialized or not
    bool initialized = false;

    // Time left
    float detectLeft = 0;
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

    private void updateAwareAnimals()
    {
        for (int i = 0; i < predatorTags.Length; i++)
        {
            GameObject[] intTagPred = GameObject.FindGameObjectsWithTag(predatorTags[i]);
            for (int z = 0; z < intTagPred.Length; z++)
            {
                if (customMathf.distanceBetweenPoints(gameObject.transform.position, intTagPred[z].transform.position) > distanceUntilDetect)
                {
                    if (baseAnimal.predatorsAwareOf.ContainsKey(intTagPred[z]))
                    {
                        baseAnimal.predatorsAwareOf.Remove(intTagPred[z]);
                    }
                    if (predatorWithinField.Contains(intTagPred[z]))
                    {
                        predatorWithinField.Remove(intTagPred[z]);
                    }
                }
                else
                {
                    if (!baseAnimal.predatorsAwareOf.ContainsKey(intTagPred[z]))
                    {
                        addPredator(intTagPred[z]);
                    }
                }
            }
        }
        for (int i = 0; i < preyTags.Length; i++)
        {
            GameObject[] intTagPrey = GameObject.FindGameObjectsWithTag(preyTags[i]);
            for (int z = 0; z < intTagPrey.Length; z++)
            {
                if (customMathf.distanceBetweenPoints(gameObject.transform.position, intTagPrey[z].transform.position) > distanceUntilDetect)
                {
                    if (baseAnimal.preyAwareOf.ContainsKey(intTagPrey[z]))
                    {
                        baseAnimal.preyAwareOf.Remove(intTagPrey[z]);
                    }
                    if (preyWithinField.Contains(intTagPrey[z]))
                    {
                        preyWithinField.Remove(intTagPrey[z]);
                    }
                }
                else
                {
                    if (!baseAnimal.preyAwareOf.ContainsKey(intTagPrey[z]))
                    {
                        addPrey(intTagPrey[z]);
                    }
                }
            }
        }
    }

    public void Update()
    {

        if (initialized)
        {
            detectLeft -= Time.deltaTime;
            if(detectLeft < 0)
            {
                detectLeft = detectTime;
                updateAwareAnimals();
            }

            // Constantly scans for 
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
