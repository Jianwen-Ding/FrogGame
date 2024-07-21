using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restartScript : MonoBehaviour
{
    public void quit()
    {
        PlayerPrefs.DeleteAll();
    }
}
