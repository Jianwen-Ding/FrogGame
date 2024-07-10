using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{

    [Header("Climbing")]
    public float climbSpeed;
    private bool isClimbing;

    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool isWallFront;

    //[Header("Keybinds")]
    //public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Climb Cooldown")]
    public float maxClimbTimer;
    private float climbTimer;


    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public OdeionPlayer P;
    public LayerMask Wall;
    public OdeionPlayerCam cam;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WallCheck();
        StateMachine();

        if (isClimbing)
            ClimbingMovement();
    }

    private void StateMachine()
    {
        // State 1 - Climbing
        if (isWallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0)
                StartClimbing();

            // timer
            if (climbTimer > 0)
                climbTimer -= Time.deltaTime;
            if (climbTimer < 0)
                StopClimbing();
        }

        // State 3 - none
        else
        {
            if (isClimbing)
                StopClimbing();
        }
    }

    private void WallCheck()
    {
        isWallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, Wall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (P.isGrounded)
            climbTimer = maxClimbTimer;
    }

    private void StartClimbing()
    {
        isClimbing = true;
        P.isClimbing = true;

    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        isClimbing = false;
        P.isClimbing = false;

    }
}