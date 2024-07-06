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
    float timeUntilNextStun;
    [SerializeField]
    float rechargeTimeLeft = 0;
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if(rechargeTimeLeft <= 0)
        {
            AnimalPresent animalAttempt = other.GetComponent<AnimalPresent>();
            if(animalAttempt != null)
            {
                animalAttempt.stunAnimal(stunTime, constTime);
                rechargeTimeLeft = timeUntilNextStun;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (rechargeTimeLeft <= 0)
        {
            AnimalPresent animalAttempt = other.GetComponent<AnimalPresent>();
            if (animalAttempt != null)
            {
                animalAttempt.stunAnimal(stunTime, constTime);
                rechargeTimeLeft = timeUntilNextStun;
            }
        }
    }
    private void Update()
    {
        rechargeTimeLeft -= Time.deltaTime;
    }
}
