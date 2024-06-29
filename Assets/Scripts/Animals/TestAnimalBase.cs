using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimalBase : AnimalPresent
{
    [SerializeField]
    float speed;
    [SerializeField]
    float hopForce;
    [SerializeField]
    float hopTime;
    float timeLeft = 0;
    public override void movementUpdate()
    {
        if (detector.predatorWithinField.Count > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0){
                timeLeft = hopTime;
                Vector3 diff = gameObject.transform.position - playerObject.transform.position;
                diff = diff.normalized;
                diff = new Vector3(diff.x * speed, hopForce, diff.z* speed);
                animalRigid.MoveRotation(Quaternion.Euler(new Vector3(0, 90 - Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg, 0)));
                animalRigid.AddForce(diff);
            }
        }
        else
        {
            Vector3 diff = gameObject.transform.position - playerObject.transform.position;
            diff = diff.normalized;
            animalRigid.MoveRotation(Quaternion.Euler(new Vector3(0, -90 - Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg, 0)));
        }
    }
}
