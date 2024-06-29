using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class researchSys : MonoBehaviour
{
    #region psuedocode
    // The research is comprised of three parts
    // 1 - Clues - Clues activate information, certain clues are locked by other information
    // 2 - Information - If a clue is discovered and all the prerequisite information is found, the corresponding
    // Information piece is activated. This information peice is logged in speciestopia and allows the player to analyze other previously
    // Locked clues.
    // 3 - Final - Once a certain set of information is found the missions is complete and the player
    // is give a overview of the situation and/or a extra tool in regards to catching the animal.
    #endregion
    #region vars
    [SerializeField]
    Dictionary<string, bool> information = new Dictionary<string, bool>();
    [SerializeField]
    string[] requiredInformation;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
