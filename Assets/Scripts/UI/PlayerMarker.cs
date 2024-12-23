using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    // This class controls the 
    #region variables
    // >>> Cache Variables <<<
    // Represents the dimensions of the parent minimap image
    private float leftX;
    private float rightX;
    private float bottomY;
    private float topY;
    private float xLength;
    private float yHeight;
    private GameObject player;
    // >>> Parameters <<<
    // The player x-z position that cooresponds to the center of the minimap
    [SerializeField]
    Vector2 centerCoord;
    // How a change in the minimap x position corresponds to a change in x position in the game corresponds
    [SerializeField]
    float xScale;
    // HHow a change in the minimap z position corresponds to a change in z position in the game corresponds
    [SerializeField]
    float zScale;
    #endregion  

    // Scales a position relative to the size of the parent minimap 
    // to one in real UI coordinates
    // (1,1) logicPos would translate to top right of the mini map
    // (-1,-1) logicPos would translate to bottom left of the mini map
    Vector2 logicToUIPos(Vector2 logicPos){
        Vector2 retPos = new Vector2();
        retPos.x = leftX +  (xLength / 2) + (xLength / 2) * logicPos.x;
        retPos.y = bottomY +  (yHeight / 2) + (yHeight / 2) * logicPos.y;
        return retPos;
    }

    // Scales a game position into logical coordinates set relative to the size of the parent minimap object
    // (1,1) logicPos would translate to top right of the mini map
    // (-1,-1) logicPos would translate to bottom left of the mini map
    Vector2 gameToLogicPos(Vector2 gamePos){
        Vector2 centeredPos = gamePos - centerCoord;
        Vector2 adjustedPos = new Vector2(centeredPos.x / xScale, centeredPos.y / zScale);
        return adjustedPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting up relavent coordinates
        Vector3[] givenCoords = new Vector3[4];
        ((RectTransform) transform.parent).GetWorldCorners(givenCoords);
        leftX = givenCoords[0].x;
        rightX = givenCoords[2].x;
        bottomY = givenCoords[0].y;
        topY = givenCoords[2].y;
        xLength = rightX - leftX;
        yHeight = topY - bottomY;

        // Finds player to represents
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        gameObject.transform.position = logicToUIPos(gameToLogicPos(playerPos));
    }
}
