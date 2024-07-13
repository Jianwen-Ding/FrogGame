using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawnerScript : MonoBehaviour
{
    // This script spawns in animals at start of scene
    // TBD- modify spawned amount based on certain quest completions
    [SerializeField]
    int baseAmount;
    [SerializeField]
    GameObject animalRadiiprefab;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < baseAmount; i++)
        {
            Instantiate(animalRadiiprefab, gameObject.transform.position, Quaternion.identity.normalized);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
