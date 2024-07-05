using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTool : MonoBehaviour
{
    #region variables
    // Time that the animal is stunned for
    [SerializeField]
    float stunTime;
    [SerializeField]
    float timeUntilNextStun;
    float rechargeTimeLeft = 0;
    #endregion
    private void OnTriggerEnter(Collider other)
    {
    }
    private void Update()
    {
        
    }
}
