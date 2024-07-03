using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBase : AnimalPresent
{
    [SerializeField]
    float speed;
    [SerializeField]
    float hopForce;
    [SerializeField]
    float panicHopTime;
    [SerializeField]
    float huntHopTime;
    [SerializeField]
    float wanderHopTime;
    [SerializeField]
    float moveHopTime;
    float hopTimeLeft = 0;
    private void hop(float angle)
    {
        Vector3 force = customMathf.angleToPoint(angle, speed);
        force.y = hopForce;
        animalRigid.MoveRotation(Quaternion.Euler(new Vector3(0, 90 - angle, 0)));
        animalRigid.AddForce(force);
        faceAngle = angle;
    }
    // Moves away from predator
    public override void panicMovementUpdate()
    {
        hopTimeLeft -= Time.deltaTime;
        if (hopTimeLeft <= 0)
        {
            GameObject closestPred = customMathf.findGreatestKeys(predatorsAwareOf, (pred) => -(pred.transform.position - gameObject.transform.position).magnitude, null);
            hopTimeLeft = panicHopTime;
            Vector3 diff = gameObject.transform.position - closestPred.transform.position;
            float getAngle = customMathf.pointToAngle(diff.x, diff.z);
            hop(getAngle);
        }
    }
    // Moves towards prey
    public override void huntMovementUpdate()
    {
        hopTimeLeft -= Time.deltaTime;
        if (hopTimeLeft <= 0)
        {
            GameObject closestPrey = customMathf.findGreatestKeys(preyAwareOf, (prey) => -(prey.transform.position - gameObject.transform.position).magnitude, null);
            hopTimeLeft = huntHopTime;
            Vector3 diff = closestPrey.transform.position - gameObject.transform.position;
            float getAngle = customMathf.pointToAngle(diff.x, diff.z);
            hop(getAngle);
        }
    }
    // Randomly hops when no directive
    public override void wanderMovementUpdate()
    {
        hopTimeLeft -= Time.deltaTime;
        if (hopTimeLeft <= 0)
        {
            hopTimeLeft = wanderHopTime;
            float getAngle = Random.Range(0, 360);
            hop(getAngle);
        }
    }
    // Moves in a specific direction following player radii
    public override void moveMovementUpdate()
    {
        hopTimeLeft -= Time.deltaTime;
        if (hopTimeLeft <= 0)
        {
            hopTimeLeft = moveHopTime;
            hop(preManifestDirection);
        }
    }

}
