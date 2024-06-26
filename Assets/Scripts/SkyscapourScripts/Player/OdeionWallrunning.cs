using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdeionWallrunning : MonoBehaviour
{

    [Header("Wallrunning")]
    public LayerMask Wall;
    public LayerMask Ground;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallJumpFrontForce;
    public float wallJumpBackForce;
    public float maxWallRunTime;
    public float wallRunTimer;
    public Vector3 wallNormal;
    public Vector3 wallForward;
    public Vector3 currentWall; 
    

    [Header("Input")]
    private float hInput;
    private float vInput;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode wallJumpKey = KeyCode.LeftControl;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit frontWallHit;
    public bool isWallLeft;
    public bool isWallRight;
    public bool isWallFront;
    public bool didFrontWallJump;

    [Header("Exiting")]
    public float exitWallTime;
    private float exitWallTimer;
    private bool isExitingWall;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Animator")]
    public Animator anim;
    //public Animator animBody;
    //private int wallRunningHash;
    private int zVelHash;

    [Header("References")]
    public Transform orientation;
    public OdeionPlayerCam cam;
    private OdeionPlayer P;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        P = GetComponent<OdeionPlayer>();

        zVelHash = Animator.StringToHash("Z_Velocity");
        

        didFrontWallJump = false;
    }

    // Update is called once per frame
    void Update()
    {

        CheckForWall();
        StateMachine();

        if (P.isGrounded)
            didFrontWallJump = false;
    }

    private void FixedUpdate()
    {
     
        if (P.isWallRunning)
        {
            WallRunningMovement();
            P.SetAnimationGrounding();
        }
    }

    public void SetAnimationWallRunning()
    {
        anim.SetBool(P.wallRunLeftHash, !isWallLeft);
        anim.SetBool(P.wallRunLeftHash, isWallLeft);
        anim.SetBool(P.wallRunRightHash, !isWallRight);
        anim.SetBool(P.wallRunRightHash, isWallRight);
        //animBody.SetBool(P.wallRunLeftHash, !isWallLeft);
        //animBody.SetBool(P.wallRunLeftHash, isWallLeft);
        //animBody.SetBool(P.wallRunRightHash, !isWallRight);
        //animBody.SetBool(P.wallRunRightHash, isWallRight);
    }

    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, Wall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, Wall);
        isWallFront = Physics.Raycast(transform.position, orientation.forward, out frontWallHit, wallCheckDistance, Wall);
    
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }

    private void StateMachine()
    {
        //Inputs
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        //wall jump facing wall
        if (Input.GetKeyDown(wallJumpKey) && isWallFront && AboveGround()) 
            FrontWallJump();
       
        //State 1 - Wallrunning
        if ((isWallLeft || isWallRight) && Input.GetKey(jumpKey) && AboveGround() && !isExitingWall)
        {
            if (!P.isWallRunning)
            {
                StartWallRun();
                //SetAnimationWallRunning();
            }

            //wall jump
            if (Input.GetKeyDown(wallJumpKey))
            {
                WallJump();
                //SetAnimationWallRunning();
                //P.SetAnimationGrounding();
            }
                
        }
        

         //State 2 - Exiting Wallrun
        else if (isExitingWall)
        {
            if (!P.isWallRunning)
            {
                StopWallRun();
                //SetAnimationWallRunning();
                //P.SetAnimationGrounding();
            }

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                isExitingWall = false;
        }

        //State 3 - none
        else
        {
            if (P.isWallRunning)
            {
                StopWallRun();
                //SetAnimationWallRunning();
            }
        }


    }

    private void StartWallRun()
    {
        P.isWallRunning = true;
        SetAnimationWallRunning();

        //Apply camera effects
        cam.DoFov(P.fov * 0.15f + P.fov);
        if (isWallLeft && P.isWallRunning)
            cam.DoTilt(-5f);
        if (isWallRight && P.isWallRunning)
            cam.DoTilt(5f);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (wallRunTimer <= 0)
            StopWallRun(); 

        wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        

        wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
            //currentWall = wallForward; 
        }

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

         //push to wall force
        if (!(isWallLeft && Input.GetKeyDown(jumpKey)) && !(isWallRight && Input.GetKeyDown(jumpKey)))
        {
            //viewDirection = Vector3.Cross(wallNormal, cam);
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
            //rb.AddForce(viewDirection * 100, ForceMode.Force);
        }

        //weaken wall gravity
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);

    }
    
    private void StopWallRun()
    {
        //rb.useGravity = true;
        P.isWallRunning = false;
        //SetAnimationWallRunning();
        anim.SetBool(P.wallRunLeftHash, false);
        anim.SetBool(P.wallRunRightHash, false);
        //animBody.SetBool(P.wallRunLeftHash, false);
        //animBody.SetBool(P.wallRunRightHash, false);
        //P.SetAnimationGrounding();
        

        currentWall = new Vector3(0f, 0f, 0f);

        //reset camera effects
        cam.DoFov(P.fov);
        cam.DoTilt(0f);

    }

    private void WallJump()
    {
        //begin exiting wall
        isExitingWall = true;
        exitWallTimer = exitWallTime;

        anim.SetBool(P.wallRunLeftHash, false);
        anim.SetBool(P.wallRunRightHash, false);
        //animBody.SetBool(P.wallRunLeftHash, false);
        //animBody.SetBool(P.wallRunRightHash, false);

        //SetAnimationWallRunning();
        //P.SetAnimationGrounding();

        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //resets the y velocity and adds forrce
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    public void FrontWallJump()
    {
        //begin exiting wall
        //isExitingWall = true;
        //exitWallTimer = exitWallTime;

        //Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpFrontForce + frontWallHit.normal * wallJumpBackForce;

        //resets the y velocity and adds forrce
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(forceToApply, ForceMode.Impulse);

        didFrontWallJump = true;

        StartCoroutine(ResetDidFrontWallJump());
    }

    private IEnumerator ResetDidFrontWallJump()
    {
        yield return new WaitForSeconds(0.1f);

        didFrontWallJump = false;
    }
}
