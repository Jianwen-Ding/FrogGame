using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class despawnScript : MonoBehaviour
{
    // Despawns an object in a set amount of time
    [SerializeField]
    float despawnTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        despawnTime -= Time.deltaTime;
        if(despawnTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
