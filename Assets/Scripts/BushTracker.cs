using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushNode
{
    float distance;
    Vector3 position;

    public BushNode(float dist, Vector3 pos)
    {
        distance = dist;
        position = pos;
    }

    public bool nearEnough(Vector3 pos)
    {
        Vector3 diff = position - pos;
        return distance > diff.magnitude;
    }
}

public class BushTracker : MonoBehaviour
{
    [SerializeField]
    float distanceScale;

    List<BushNode> bushNodes;

    [SerializeField]
    float updateTime;

    float updateLeft;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] bushObjects = GameObject.FindGameObjectsWithTag("Bush");
        bushNodes = new List<BushNode>();
        for(int i = 0; i < bushObjects.Length; i++)
        {
            GameObject bushOb = bushObjects[i];
            float dist = distanceScale * Mathf.Max(bushOb.transform.lossyScale.x, bushOb.transform.lossyScale.y, bushOb.transform.lossyScale.z);
            Vector3 pos = bushOb.transform.position;
            bushNodes.Add(new BushNode(dist, pos));
        }

        updateLeft = 0;
    }

    // Update is called once per frame
    void Update()
    {
        updateLeft += Time.deltaTime;
        if(updateLeft < updateTime)
        {
            updateLeft = 0;
            bool closeEnough = false;
            for(int i = 0; i < bushNodes.Count; i++)
            {
                if (bushNodes[i].nearEnough(gameObject.transform.position))
                {
                    closeEnough = true;
                    print("wow");
                    break;
                }
            }
        }
    }
}
