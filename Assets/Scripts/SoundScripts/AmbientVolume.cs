using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientVolume : MonoBehaviour
{
    private Transform listenerTransform;
    private AudioSource audioSource;
    public float minDist=1;
    public float maxDist=400;
    void Start()
    {
        listenerTransform = GameObject.Find("Player").GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, listenerTransform.position);
 
        if(dist < minDist)
        {
            audioSource.volume = 1;
        }
        else if(dist > maxDist)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = 1 - ((dist - minDist) / (maxDist - minDist));
        }
    }
}
