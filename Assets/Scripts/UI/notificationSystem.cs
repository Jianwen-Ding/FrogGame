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
        }
    }
    #endregion
}
