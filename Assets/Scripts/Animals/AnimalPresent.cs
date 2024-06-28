using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPresent : MonoBehaviour
{
    // This class controls a manifested animal
    // It is spawned in by the AnimalRadii class
    // This is meant to be a parent class to be inherited
    // By more specialized movement

    #region vars
    // >>> CACHE PARAMETERS <<<
    // Stores objects to be used throughout lifetime of object

    [Header("Cache Parameters")]
    public AnimalRadii animalController;
    [SerializeField]
    Renderer animalRender;
    [SerializeField]
    GameObject playerObject;

    // >>> BEHAVIOR PARAMETERS <<<
    // Behaviors set to differential overall behavior between species

    [Header("Predator prey Parameters")]
    // Tags of animals that this animal considers prey
    public string[] preyTags;
    // Tags of animals that this animal considers predator
    public string[] predatorTags;
    
    [Header("Detection Parameters")]
    // How wide the animal's field of view is 
    [SerializeField]
    float fovAngle;
    // How far the animal will be able to see
    [SerializeField]
    float detectDistance;
    // What the animal can see through
    [SerializeField]
    float sightLayerMask;

    [Header("Overarching")]
    // Whether the Animal Radii was moving before
    [SerializeField]
    bool preManifestMoving;
    // Direction that the radii was going pre
    // manifestation on the x,z plane
    [SerializeField]
    float preManifestDirection;
    // Distance until animal attempts reconnection with radii
    [SerializeField]
    float distanceUntilDeload;

    // >>> State Vars <<<
    // Variables that change constantly to record state
    // of animal

    bool initialized = false;

    #endregion
    #region functions
    // Initializes the class
    public virtual void init(bool moving ,float previousDirection, AnimalRadii setController)
    {
        initialized = true;
        preManifestMoving = moving;
        animalController = setController;
        preManifestDirection = previousDirection;
        animalRender = gameObject.GetComponent<Renderer>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Checks if object can be seen
    public bool inView()
    {
        return animalRender.isVisible;
    }
    // Signals to animal radii that the animal is ready to rejoin the radii
    public virtual void rejoinRadii()
    {
        animalController.removeManifestedAnimal(this);
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = customMathf.distanceBetweenPoints(gameObject.transform.position, playerObject.transform.position);
        if (distance > distanceUntilDeload && !inView())
        {
            rejoinRadii();
        }
    }
    #endregion
}
