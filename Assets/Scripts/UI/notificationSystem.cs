using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class notificationSystem : MonoBehaviour
{
    // This controls how on screen notifications are displayed in game
    #region variables
    // Prefab of notification
    public GameObject notifyPrefab;

    public static notificationSystem soleSystem;

    public float yAdjust;
    public List<GameObject> createdObjects = new List<GameObject>();
    #endregion
    #region functions
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindObjectsOfType<notificationSystem>().Length >= 2)
        {
            Destroy(gameObject);
        }
        else
        {
            soleSystem = this;
        }
    }

    public static void notify(string notifyText)
    {
        if(soleSystem != null)
        {
            GameObject newNotification = Instantiate(soleSystem.notifyPrefab, soleSystem.transform.position, Quaternion.identity.normalized);
            newNotification.transform.SetParent(soleSystem.transform);
            newNotification.GetComponent<TextMeshProUGUI>().text = notifyText;
            soleSystem.createdObjects.Add(newNotification);
        }
    }

    private void Update()
    {
        for(int i = 0; i < createdObjects.Count; i++)
        {
            if (createdObjects[i] == null)
            {
                createdObjects.RemoveAt(i);
            }
            else{
                createdObjects[i].transform.position = new Vector3(createdObjects[i].gameObject.transform.position.x, gameObject.transform.position.y - yAdjust * i, createdObjects[i].gameObject.transform.position.z);
            }
        }
    }
    #endregion
}
