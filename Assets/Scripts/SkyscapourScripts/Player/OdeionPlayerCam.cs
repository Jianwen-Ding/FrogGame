using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OdeionPlayerCam : MonoBehaviour
{
    public float sensitivity = 2.0f;


    //public Transform MyPlayerPivot;

    public Transform orientation;
    public GameObject CamRecoil;
    public GameObject FrogBody;
    public GameObject FrogBody3rd;
    public GameObject FrogBody3rdWeapon;
    public Camera WeaponCam;
    public Transform camHolder;
    public Transform camRoot;
    public Rigidbody rb;

    float xRotation;
    float yRotation;
    
    // Start is called before the first frame update
    public void Start()
    {
        CamRecoil.gameObject.SetActive(true);
        WeaponCam.gameObject.SetActive(true);
        //FrogBody3rd.GetComponent<Renderer>().enabled = false;
        //FrogBody3rd.gameObject.SetActive(false);
        FrogBody3rdWeapon.gameObject.SetActive(false);
            

        transform.position = camRoot.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (DialogueManager2.GetInstance().dialogueIsPlaying)
        {
            return;
        }
        else
        {
            //if(base.IsOwner)
            //FrogBody.GetComponent<Renderer>().enabled = false;
            //mouse inputs
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.smoothDeltaTime * sensitivity * 10;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.smoothDeltaTime * sensitivity * 10;
        

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
            //rotate camera and player rigidbody
    
            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            camRoot.transform.rotation = camHolder.rotation;

            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            rb.MoveRotation(Quaternion.Euler(0, yRotation, 0));
            camHolder.transform.position = camRoot.transform.position;
        }
        

    }
    void LateUpdate()
    {

    }
    
    void FixedUpdate()
    {
        
    }
    
    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public void DoXTilt(float xTilt)
    {
        transform.DOLocalRotate(new Vector3(xTilt, 0, 0), 0.25f);
    }

}
