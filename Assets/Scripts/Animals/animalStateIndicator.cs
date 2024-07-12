using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalStateIndicator : MonoBehaviour
{
    // This class controls how animal states will be shown
    #region variables
    // >>> Art Assets / Visualization Variable <<<
    // The different materials applied on each different state of an animal
    [SerializeField]
    Material stunMat;
    [SerializeField]
    Material huntMat;
    [SerializeField]
    Material panicMat;
    [SerializeField]
    Material markedMat;
    [SerializeField]
    float distanceAbove;


    // >>> Cache Variables <<<
    // Finds player in order to constantly be angled towards player
    GameObject playerOb;
    // Gets animal script in order to accurately judge what materials to show
    [SerializeField]
    AnimalPresent animalScript;
    // Renderer to put materials on
    Renderer render;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerOb = GameObject.FindGameObjectWithTag("Player");
        animalScript = transform.parent.GetComponent<AnimalPresent>();
        transform.SetParent(null);
        render = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(animalScript != null)
        {
            Vector3 diff = playerOb.transform.position - gameObject.transform.position;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(90, 90 - customMathf.pointToAngle(diff.x, diff.z), 0));
            gameObject.transform.position = animalScript.transform.position + Vector3.up * distanceAbove;
            if (animalScript.marked)
            {
                render.enabled = true;
                render.material = markedMat;
            }
            else
            {
                if (animalScript.currentState == AnimalPresent.animalState.Panic)
                {
                    render.enabled = true;
                    render.material = panicMat;
                }
                else if (animalScript.currentState == AnimalPresent.animalState.Hunt)
                {
                    render.enabled = true;
                    render.material = huntMat;
                }
                else if (animalScript.currentState == AnimalPresent.animalState.Stun)
                {
                    render.enabled = true;
                    render.material = stunMat;
                }
                else
                {
                    render.enabled = false;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
