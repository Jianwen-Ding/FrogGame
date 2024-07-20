using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notificaionDespawn : MonoBehaviour
{
    // This class controls the 
    [SerializeField]
    float xFlyoffSpeed;
    [SerializeField]
    float xFlyoffAccel;
    [SerializeField]
    float timeUntilDespawn;

    // Update is called once per frame
    void Update()
    {
        xFlyoffSpeed += Time.deltaTime * xFlyoffAccel;
        transform.position += Vector3.right * (xFlyoffSpeed) * Time.deltaTime;
        timeUntilDespawn -= Time.deltaTime;
        if(timeUntilDespawn <= 0)
        {
            Destroy(gameObject);
        }
    }
}
