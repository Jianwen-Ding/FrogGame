using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdeionSliding : MonoBehaviour
{
    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideTimer;
    private Vector3 inputDirection;
    public float slideYScale;
    private float startYScale;
    public bool canSlideAgain;
    public float canSlideTimer;

    [Header("Keybinds")]
    public KeyCode slideKey = KeyCode.LeftControl;
    public float hInput;
    private float vInput;

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private OdeionPlayer P;
    public OdeionPlayerCam cam;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        P = GetComponent<OdeionPlayer>();

        canSlideAgain = true;
        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {

        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && canSlideAgain && !P.isSliding && (hInput != 0  || vInput != 0))
            StartSlide();
        
    }

    void FixedUpdate()
    {

        if(P.isSliding)
        {
            SlidingMovement();
            //cam.camHolder.transform.position = cam.camRoot.transform.position;
        }
    }


    private void StartSlide()
    {
        if (!P.isWallRunning && vInput != -1)
        {
            P.isSliding = true;
            canSlideAgain = false;
            inputDirection = orientation.forward * vInput + orientation.right * hInput;
            
            if (P.isGrounded)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }

            cam.DoXTilt(-5f);
            if (hInput == -1)
                cam.DoTilt(-5f);
            if (hInput == 1)
                cam.DoTilt(5f);
            cam.DoFov((P.fov * 0.1f + P.fov));


            

            slideTimer = maxSlideTime;
            P.SetAnimationSliding();
        }
    }



    private void SlidingMovement()
    {
        // Normal Slide
        if (!P.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        
        // Sliding down slopes
        else
        {
            rb.AddForce(P.GetSlopeDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        //P.anim.SetBool(P.slidingHash, P.isSliding);
        
        if (slideTimer < 0)
        {
            StopSlide(); 
        }
    }

    private void StopSlide()
    {
        P.isSliding = false;
        P.SetAnimationSliding();
        //P.anim.SetBool(P.slidingHash, !P.isSliding);

        if (!P.isWallRunning && !P.isDashing)
        {
            cam.DoFov(P.fov);
            cam.DoXTilt(0f);
            cam.DoTilt(0f);
            
        }

        StartCoroutine(waitToSlide());
    }

    private IEnumerator waitToSlide()
    {
        yield return new WaitForSeconds(canSlideTimer);

        canSlideAgain = true;
    }


}
