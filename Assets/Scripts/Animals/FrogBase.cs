using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FrogBase : AnimalPresent
{
    #region variables
    // >>> Behavior Variables <<<
    [Header("Behavioral Parameters")]
    [SerializeField]
    float speed;
    [SerializeField]
    float hopForce;
    [SerializeField]
    float transitionHopTime;
    [SerializeField]
    float panicHopTime;
    [SerializeField]
    float huntHopTime;
    [SerializeField]
    float wanderHopTime;
    [SerializeField]
    float moveHopTime;
    float hopTimeLeft = 0;
    // Variables for adjusting hop angle
    [Header("Collision Avoidance Parameters")]
    [SerializeField]
    LayerMask collisionLayerMask;
    [SerializeField]
    float colDetectDistance;
    [SerializeField]
    float colDetectRad;
    [SerializeField]
    int colDetectAttempts;
    [SerializeField]
    float colDetectAttemptAngleOffset;
    [SerializeField]
    float colDetectFalloff;

    #endregion

    #region functions
    // A single hop that comprises most of the movement
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
            getAngle = refactorDirection(getAngle, gameObject.transform.position, colDetectDistance, colDetectAttemptAngleOffset, colDetectAttempts, colDetectFalloff, collisionLayerMask, colDetectRad);
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
            getAngle = refactorDirection(getAngle, gameObject.transform.position, colDetectDistance, colDetectAttemptAngleOffset, colDetectAttempts, colDetectFalloff, collisionLayerMask, colDetectRad);
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
            getAngle = refactorDirection(getAngle, gameObject.transform.position, colDetectDistance, colDetectAttemptAngleOffset, colDetectAttempts, colDetectFalloff, collisionLayerMask, colDetectRad);
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
            float getAngle = refactorDirection(preManifestDirection, gameObject.transform.position, colDetectDistance, colDetectAttemptAngleOffset, colDetectAttempts, colDetectFalloff, collisionLayerMask, colDetectRad);
            hop(getAngle);
        }
    }

    // Completely resets hop time on state transition
    public override void transitionMovementUpdate()
    {
        hopTimeLeft = transitionHopTime;
    }

    // Solely for debug
    public override void Update()
    {
        base.Update();
        refactorDirection(faceAngle, gameObject.transform.position, colDetectDistance, colDetectAttemptAngleOffset, colDetectAttempts, colDetectFalloff, collisionLayerMask, colDetectRad);
    }
    #endregion
}