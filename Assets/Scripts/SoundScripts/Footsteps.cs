using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] DirtSteps;
    public AudioClip[] GrassSteps;
    public AudioClip[] LeavesSteps;
    public AudioClip[] StoneSteps;
    public AudioClip[] WoodSteps;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        int dRandom = Random.Range(0, DirtSteps.Length);
        int gRandom = Random.Range(0, GrassSteps.Length);
        int lRandom = Random.Range(0, LeavesSteps.Length);
        int sRandom = Random.Range(0, StoneSteps.Length);
        int wRandom = Random.Range(0, WoodSteps.Length);

        if (collision.CompareTag("Dirt"))
            AudioSource.PlayClipAtPoint(DirtSteps[dRandom], transform.position);
        if (collision.CompareTag("Grass"))
            AudioSource.PlayClipAtPoint(GrassSteps[gRandom], transform.position);
        if (collision.CompareTag("Leaves"))
            AudioSource.PlayClipAtPoint(LeavesSteps[lRandom], transform.position);
        if (collision.CompareTag("Stone"))
            AudioSource.PlayClipAtPoint(StoneSteps[sRandom], transform.position);
        if (collision.CompareTag("Wood"))
            AudioSource.PlayClipAtPoint(WoodSteps[wRandom], transform.position);
    }
}
