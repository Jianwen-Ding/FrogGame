using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class loadNewScene : MonoBehaviour
{
    [SerializeField]
    string id;
    public void next()
    {
        SceneManager.LoadScene(id);
    }
}
