using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTool : MonoBehaviour
{
    // This tool stuns any animals within it's radius
    // Upon it being recharged

    #region variables
    // Time that the animal is stunned for
    [SerializeField]
    float stunTime;
    // Whether animal stun multiplier is factored
    [SerializeField]
    bool constTime;
    // Recharge time for stun tool
    [SerializeField]
    float timeUntilDelete;
    [SerializeField]
    bool hasStunned = false;
    [SerializeField]
    float timeLeft = 0;
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if(hasStunned)
        {
            AnimalPresent animalAttempt = other.GetComponent<AnimalPresent>();
            if(animalAttempt != null)
            {
                animalAttempt.stunAnimal(stunTime, constTime);
                hasStunned = true;
                timeLeft = timeUntilDelete;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!hasStunned)
        {
            AnimalPresent animalAttempt = other.GetComponent<AnimalPresent>();
            if (animalAttempt != null)
            {
                animalAttempt.stunAnimal(stunTime, constTime);
                hasStunned = true;
                timeLeft = timeUntilDelete;
            }
        }
    }
    private void Update()
    {
        if (hasStunned)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
