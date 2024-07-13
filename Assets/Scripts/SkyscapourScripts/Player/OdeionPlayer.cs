using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class OdeionPlayer : MonoBehaviour
{

    [Header("Objects")]
    [SerializeField] private GameObject coinPic;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode UtilityKeyOne = KeyCode.Q;
    public KeyCode UtilityKeyTwo = KeyCode.E;

    [Header("Ground Check")]
    public float playerHeight;
    private float groundDistance = 0.4f;
    public LayerMask Ground;
    public bool isGrounded;
    public float groundDrag;

    [Header("Slope Check")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool leaveSlope;

    [Header("Player Parameters")]
    public float gravity;
    float hInput;
    float vInput;
    public float health;
    //private int maxAmmo = 50; 
    private bool isReloading = false;
    public bool hasCoin = false;
    [SerializeField] private int ammo; 
    public Collider coll;
    public float fov;


    [Header("Movement")]
    public bool isMoving;
    public float speed;
    public float desiredSpeed;
    private float lastDesiredSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    private Vector3 direction;
    public float slideSpeed;
    public float wallrunSpeed;
    public float dashSpeed;
    public float maxYSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    
    private MovementState lastState;
    private bool keepMomentum;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    public bool cantUncrouch;

    [Header("Dashing")]
    public bool isDashing;
    public float dashSpeedChangeFactor;

    [Header("Wallrun")]
    //public LayerMask Wall;
    public bool isWallRunning;

    [Header("Slide")]
    //public LayerMask Wall;
    public bool isSliding;

    [Header("Climbing")]
    //public LayerMask Wall;
    public Climbing climbScript;
    public bool isClimbing;
    public float climbSpeed;

    [Header("Equipment Items")]
    //Double Dash ability
    public bool doubleTime;
    //Double Sprint ability
    public bool allegro;

    [Header("Utility Items")]
    public GameObject UtilityItemOne;
    public GameObject UtilityItemTwo;
    public bool isForkussed;
    public float untilThrowRecharge;
    public float untilThrowRechargeLeft;
    public float throwForce = 20f;
    public float tEffectDuration;
    public float tMaxEffectDuration = 1f;
    public float tSpeedModifier = 0.3f;


    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    public Rigidbody rb;
    public OdeionPlayerCam cam;
    public Camera PlayerViewCam;


    [Header("Post Processing")]
    public PostProcessVolume fk;
    public PostProcessVolume dEffect;
    private float dNumber;
    public bool isDamaged;
    public float dEffectDuration;
    public float dMaxEffectDuration = 0.5f;
    private float dW; 
    private float w;

    [Header("Animator")]
    public Animator anim;
    //public Animator animBody;
    private bool hasAnimator;
    private int xVelHash;
    private int yVelHash;
    private int zVelHash;
    private Vector2 currentVel;
    public float AnimationBlendSpeed = 8.9f;
    private int jumpHash;
    private int fallingHash;
    private int groundHash;
    public int slidingHash;
    public int wallRunLeftHash;
    public int wallRunRightHash;
    public int crouchingHash;


    [Header("Managers")]
    private UIManager UI;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        sliding,
        crouching,
        dashing,
        climbing,
        air
    }

    [Header("Sound Modifiers")]
    // This controls how other animals hear the player in its different states
    // Order from standing, walking, sprinting, wallrunning, sliding, crouching, dashing, air
    [SerializeField]
    float[] soundMultipliers = new float[8];
    [SerializeField]
    float standingVelMag;

    public void Start()
    {
        Cursor.visible = false; //hide cursor
        Cursor.lockState = CursorLockMode.Locked; //unlock

         //sets default yscale of player
        startYScale = transform.localScale.y;

        //hasAnimator = TryGetComponent<Animator>(out anim);
        xVelHash = Animator.StringToHash("X_Velocity");
        yVelHash = Animator.StringToHash("Y_Velocity");
        zVelHash = Animator.StringToHash("Z_Velocity");
        jumpHash = Animator.StringToHash("Jump");
        fallingHash = Animator.StringToHash("Falling");
        groundHash = Animator.StringToHash("Grounded");
        slidingHash = Animator.StringToHash("Sliding");
        //wallRunningHash = Animator.StringToHash("WallRunning");
        wallRunLeftHash = Animator.StringToHash("WallRunningLeft");
        wallRunRightHash = Animator.StringToHash("WallRunningRight");
        crouchingHash = Animator.StringToHash("Crouched");
        //harmonicRifleHash = Animator.StringToHash("HarmonicRifle");
        //bassoonArmCannonHash = Animator.StringToHash("BassoonArmCannon");

        rb.freezeRotation = true;
        readyToJump = true;

        cam.DoFov(fov);

    }
    
    // Update is called once per frame
    void Update()
    {
        //if (isForkussed)
            //ForkussionEffect();
        //ForkussionEnd();
        if (isDamaged)
            DamageEffect();
        if (dEffect.weight > 0)
            DamageEnd();
        
        SpeedController();
        StateHandler();
        Jumping();
        Crouching();
        MyInput();
        ThrowUtility();

        //ResetJumpTwo();

        untilThrowRechargeLeft -= Time.deltaTime;

        //WallCling();
        
        if (isGrounded && !isDashing)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    void FixedUpdate() //linked to clock time
    {
        //if (DialogueManager2.GetInstance().dialogueIsPlaying)
        //{
        //    return;
        //}
        
        //MyInput();
        Move();
        
        //establishes gravity
        if (!OnSlope())
            rb.AddForce(Vector3.down * gravity * rb.mass);
        //if (isGrounded && readyToJump && !OnSlope())
            //rb.velocity = new Vector3(rb.velocity.x, -1f, rb.velocity.z);
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        if (OnSlope())
            isGrounded = true;
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, playerHeight/2, 0), groundDistance, Ground);
        
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //show cursor
            Cursor.visible = true; 
            Cursor.lockState = CursorLockMode.None; //unlock
        }
        
        
    }

    // Gives sound multiplier of the character
    public float getCurrentSoundMultiplier()
    {
        switch (state)
        {
            case MovementState.walking:
                if(rb.velocity.magnitude < standingVelMag)
                {
                    return soundMultipliers[0];
                }
                else
                {
                    return soundMultipliers[1];
                }
            case MovementState.sprinting:
                return soundMultipliers[2];
            case MovementState.wallrunning:
                return soundMultipliers[3];
            case MovementState.sliding:
                return soundMultipliers[4];
            case MovementState.crouching:
                return soundMultipliers[5];
            case MovementState.dashing:
                return soundMultipliers[6];
            case MovementState.air:
                return soundMultipliers[7];
        }
        return 0;
    }

    void ThrowUtility()
    {
        if (untilThrowRechargeLeft <= 0 && Input.GetKeyDown(UtilityKeyOne) && !Input.GetKey(UtilityKeyTwo))
        {
            untilThrowRechargeLeft = untilThrowRecharge;
            GameObject UtilityItem = Instantiate(UtilityItemOne, playerCam.position, transform.rotation);
            UtilityItem.GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce, ForceMode.Impulse);

        }
        if(untilThrowRechargeLeft  <= 0 && Input.GetKeyDown(UtilityKeyTwo) && !Input.GetKey(UtilityKeyOne))
        {
            untilThrowRechargeLeft = untilThrowRecharge;
            GameObject UtilityItem = Instantiate(UtilityItemTwo, playerCam.position, transform.rotation);
            UtilityItem.GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce, ForceMode.Impulse);

        }
    }

    public void ForkussionEffect()
    {
         
        //if (w > 0 && isForkussed)
        //{
            tEffectDuration = tMaxEffectDuration;
            w += Time.deltaTime * 5;
            fk.weight = w;
            Debug.Log("Stuff");
            if (w >= 1)
            {
                speed = speed * tSpeedModifier;
                isForkussed = false;
                //w -= Time.deltaTime;
                //fk.Weight.Override(w);
                
            }
        
    }

    public void ForkussionEnd()
    {
        if(tEffectDuration > 0)
        {
            tEffectDuration -= Time.deltaTime;
            
        }
        if (!isForkussed && w > 0 && tEffectDuration <= 0)
            {
                w -= Time.deltaTime / 2;
                fk.weight = w;
                if (w <= 0)
                {
                    w = 0;
                    fk.weight = w;
                    speed = speed / tSpeedModifier;
                    return;
                }
            }
    }



    private void Jumping()
    {
        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            
            readyToJump = false;

            Jump();

            anim.SetTrigger(jumpHash);
            
            //Invoke(nameof(ResetJump), jumpCooldown);
            //SetAnimationGrounding();
            Invoke(nameof(ResetJumpOne), jumpCooldown);
            SetAnimationGrounding();

        }
        
        /*if (rb.velocity.y <= 0 && !readyToJump && isGrounded)
        {

            Invoke(nameof(ResetJumpOne), jumpCooldown);
            SetAnimationGrounding();

        }*/

    //Falling
    anim.SetFloat(zVelHash, rb.velocity.y);
        
        SetAnimationGrounding();

    }

    public void SetAnimationGrounding()
    {
        anim.SetBool(fallingHash, !isGrounded);
        anim.SetBool(groundHash, isGrounded);

    }

    public void SetAnimationSliding()
    {
        anim.SetBool(slidingHash, !isSliding);
        anim.SetBool(slidingHash, isSliding);

    }

    private void Crouching()
    {
        cantUncrouch = Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.1f, Ground);
   
        //toggles player crouch
        if (isGrounded && Input.GetKeyDown(crouchKey))
        {
           anim.SetBool(crouchingHash, true);
        }

        if (!Input.GetKey(crouchKey) && !cantUncrouch)
        {
           anim.SetBool(crouchingHash, false);

        }

        if (state != MovementState.crouching)
        {
           anim.SetBool(crouchingHash, false);

        }
        
        /*else if (Input.GetKeyUp(crouchKey) && cantUncrouch)
            anim.SetBool(crouchingHash, true);
            animBody.SetBool(crouchingHash, true);*/


    }
    
    private void MyInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
        
    }

    private void StateHandler()
    {

        //Sets slide
        if (isSliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredSpeed = slideSpeed;
                
            }
            else    
            {
                desiredSpeed = sprintSpeed;
            }

            
        }

         //Sets climbing
        else if (isClimbing)
        {
            state = MovementState.climbing;
            desiredSpeed = climbSpeed;
            isGrounded = false;
            
        }

        //Sets crouching
        else if (Input.GetKey(crouchKey) && isGrounded)
        {
            state = MovementState.crouching;
            desiredSpeed = crouchSpeed;
        }

        //Sets dashing
        else if (isDashing && !allegro)
        {
            state = MovementState.dashing;
            desiredSpeed = dashSpeed;
            speedIncreaseMultiplier = dashSpeedChangeFactor;
        }

        //Sets sprinting
        else if (isGrounded && state != MovementState.crouching && Input.GetKey(sprintKey) && allegro)
        {
            cam.DoFov(fov * 0.15f + fov);
            state = MovementState.sprinting;
            desiredSpeed = sprintSpeed;
        }


        //Sets wallrun
        else if (isWallRunning)
        {
            state = MovementState.wallrunning;
            desiredSpeed = wallrunSpeed;
        }

        //Sets walking
        else if (isGrounded && !Input.GetKey(sprintKey))
        {
            cam.DoFov(fov);
            state = MovementState.walking;
            desiredSpeed = walkSpeed;
        }

        //Sets air
        else
        {
            state = MovementState.air;

            if (desiredSpeed < sprintSpeed)
                desiredSpeed = walkSpeed;
            else
                desiredSpeed = sprintSpeed;
        }

        // Checks if desiredspeed has changed by a large amount
        if (Mathf.Abs(desiredSpeed - lastDesiredSpeed) > 7f && speed != 0)
        {
            //StopAllCoroutines();
            StartCoroutine(SmoothlyLerpSpeed());
        }

        else
        {
            speed = desiredSpeed;
        }

        bool hasDesiredSpeedChanged = desiredSpeed != lastDesiredSpeed;

        if (lastState == MovementState.dashing)
            keepMomentum = true;
        
        if (keepMomentum)
        {
            StartCoroutine(SmoothlyLerpSpeed());
        }
        else
        {
            speed = desiredSpeed;
        }

        lastDesiredSpeed = desiredSpeed;
        lastState = state;

        //Keeps Player from Sliding
        if (vInput == 0 && hInput == 0 && rb.velocity.y < 0.1f && readyToJump == true && state != MovementState.air 
        && state != MovementState.dashing && state != MovementState.sliding && state != MovementState.wallrunning)
        {
            isMoving = false;
            coll.material.staticFriction = 1.4f;
            //rb.velocity = new Vector3(0,rb.velocity.y,0);
            //rb.velocity = new Vector3(0,rb.velocity.y,0);
            //rb.speed = 0.0f;
        }
        else 
        {
            isMoving = true;
            coll.material.staticFriction = 0.0f;
            //rb.freezeRotation = true;
        }


    }
    
    private void Move()
    {
        if (state == MovementState.dashing)
            return;
        if (state == MovementState.sliding)
            return;
        /*if (climbScript.isExitingWall)
            return;*/
        
        //Set movement
        direction = orientation.forward * vInput + orientation.right * hInput;

        currentVel.x = Mathf.Lerp(currentVel.x, hInput * speed, AnimationBlendSpeed * Time.fixedDeltaTime);
        currentVel.y = Mathf.Lerp(currentVel.y, vInput * speed, AnimationBlendSpeed * Time.fixedDeltaTime);

        
            
        //on slope
        if (OnSlope() && !leaveSlope)
        {
            rb.AddForce(GetSlopeDirection(direction) * speed * 30f, ForceMode.Force);
            anim.SetFloat(xVelHash, currentVel.x);
            anim.SetFloat(yVelHash, currentVel.y);
            

            if (rb.velocity.y > 0f)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);

        }

        //on ground
        else if (isGrounded)
        {
            rb.AddForce(direction.normalized * speed * 15f, ForceMode.Force);
            anim.SetFloat(xVelHash, currentVel.x);
            anim.SetFloat(yVelHash, currentVel.y);
            
        }
        
        
        
        //in air
        else if (!isGrounded)
            rb.AddForce(direction.normalized * speed * 15f / airMultiplier, ForceMode.Force);
        
        //turns off gravity while on slope
        rb.useGravity = !OnSlope();
        
        
    }

    private void SpeedController()
    {
        // limiting speed on slope
        /*if (OnSlope() && !leaveSlope)
        {
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;
        }*/

        //limits speed on the ground or air
        Vector3 maxSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limits velocity if necessary
        if (maxSpeed.magnitude > speed)
        {
            Vector3 limitedSpeed = maxSpeed.normalized * speed;
            rb.velocity = new Vector3(limitedSpeed.x, rb.velocity.y, limitedSpeed.z);
        }
        

        /*if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed && state == MovementState.air)
        {
            rb.velocity = new Vector3(rb.velocity.x, -maxYSpeed, rb.velocity.z);
            //if (isGrounded)
                //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }*/

        // reset y velocity
        //if (rb.velocity.y < 0f && isGrounded)
           //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    private void Jump()
    {
        if (!isGrounded)
            return;

        //allows you to jump on slopes
        leaveSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Debug.Log("Hey" + rb.velocity.y + " ");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        

    }

    private void ResetJumpOne()
    {

        Invoke(nameof(ResetJumpTwo), 0.5f);
    
    }

    private void ResetJumpTwo()
    {
        
        readyToJump = true;

        leaveSlope = false;

        Debug.Log("ayo");
    
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.4f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;

        }

        return false;
    }

    public Vector3 GetSlopeDirection(Vector3 D)
    {
        return Vector3.ProjectOnPlane(D, slopeHit.normal).normalized;
    }

    private IEnumerator SmoothlyLerpSpeed()
    {
        // Slowly changes movement speed to the desired speed value
        float time = 0;
        float difference = Mathf.Abs(desiredSpeed - speed);
        float startValue = speed;

        //float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }

            else
                time += Time.deltaTime * speedIncreaseMultiplier;
            
            yield return null;
        }

        speed = desiredSpeed;
        //speedChangeFactor = 1f;
        keepMomentum = true;
    }

    public void SetCoin(bool set)
    {
        hasCoin = set;
        coinPic.SetActive(set);
    }

    public void DamageEffect()
    {
         
        //if (w > 0 && isForkussed)
        //{
            dEffectDuration = dMaxEffectDuration;
            dW += Time.deltaTime * 5;
            dEffect.weight = dW;
            if (dW >= dNumber)
            {
                isDamaged = false;
                dEffect.weight = dNumber;
            }
                
            //Debug.Log("Stuff");
            /*if (dW >= dNumber)
            {
                //DamageEnd();
                //w -= Time.deltaTime;
                //fk.Weight.Override(w);
                
            }*/
        
    }

    public void DamageEnd()
    {
        if(dEffectDuration > 0)
        {
            dEffectDuration -= Time.deltaTime * 2;
            
        }
        if (dW > 0 && dEffectDuration <= 0)
            {
                dW -= Time.deltaTime / 2;
                dEffect.weight = dW;
                if (dW <= 0)
                {
                    dW = 0;
                    dEffect.weight = dW;
                    return;
                }
            }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        dNumber += damage * 0.01f;
        isDamaged = true;
        if (health < 0)
        {
            //remove
            Destroy(this.gameObject);
        }
    }

}
