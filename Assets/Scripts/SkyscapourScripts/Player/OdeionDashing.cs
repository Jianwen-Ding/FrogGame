using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class OdeionDashing : MonoBehaviour
{
    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float maxDashYSpeed;
    private Vector3 delayedForceToApply;
    public float hInput;
    public float vInput;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;
    
    [Header("Keybinds")]
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Dash Cooldown")]
    public float dashCooldown;
    public float dashCooldownTimer;
    public float airDashCount;
    public float maxAirDashCount;
    public float dashCount;
    public float maxDashCount;


    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private OdeionPlayer P;
    public OdeionPlayerCam cam;
    public OdeionWallrunning W;
    //public PostProcessVolume Ppv;
    //private Vignette V;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        P = GetComponent<OdeionPlayer>();
        if (P.doubleTime)
        {
            maxDashCount = 2;
            maxAirDashCount = 2;
            dashCooldown = 4;
        }
        else
        {
            maxDashCount = 1;
            maxAirDashCount = 1;
            dashCooldown = 2;
        }
        dashCount = maxDashCount;
        airDashCount = maxAirDashCount;
        //Ppv.profile.TryGetSettings(out V);
        //V.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
        if (P.allegro)
            return;
        if (Input.GetKeyDown(dashKey))
        {
            if (P.isGrounded)
                Dash();
            else if (!P.isGrounded && airDashCount > 0 || dashCooldownTimer <= 0)
            {
                Dash();
                airDashCount--;
                
            }
            
        }
        if (P.isGrounded || P.isWallRunning || W.didFrontWallJump)
        {
            airDashCount = maxAirDashCount;
        }
            

        if (dashCooldownTimer > 0 )
            dashCooldownTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (dashCooldownTimer > 0 && dashCount == 0) 
            return;
        else if (dashCooldownTimer <= 0)
        {
            dashCooldownTimer = dashCooldown;
            dashCount = maxDashCount;
        }
        dashCount--;

        P.isDashing = true;
        P.maxYSpeed = maxDashYSpeed;
        //V.active = true;
        //V.intensity.value = Mathf.Lerp(0f, 0.559f, 1.0f);
        

        cam.DoFov(P.fov * 0.2f + P.fov);
        if (hInput == -1)
            cam.DoTilt(2f);
        if (hInput == 1)
            cam.DoTilt(-2f);

        Transform forwardT;

        if (useCameraForward)
            forwardT = cam.transform; // where you're looking
        else
            forwardT = orientation; // where you're facing (no up or down)

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;
        
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        P.isDashing = false;
        P.maxYSpeed = 0;
        //V.active = false;
        //V.intensity.value = Mathf.Lerp(0.559f, 0f, 1.0f);

        

        if(!P.isWallRunning || P.isSliding)
        {
            cam.DoFov(P.fov);
            cam.DoTilt(0f);
        }
        
        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * vInput + forwardT.right * hInput;
        else
            direction = forwardT.forward;

        if (vInput == 0 && hInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }
}
