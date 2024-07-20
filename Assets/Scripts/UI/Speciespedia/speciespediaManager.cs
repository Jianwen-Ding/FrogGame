using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speciespediaManager : MonoBehaviour
{
    // This loads all of speciespedia
    [SerializeField]
    speciesPage[] pages;

    [SerializeField]
    GameObject pauseTab;
    // Start is called before the first frame update
    void Start()
    {
        speciesPage.pageList = new Dictionary<string, speciesPage>();
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].init();
        }
    }

    public void showPage(int pageIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].gameObject.SetActive(false);
        }
        if(pageIndex >= 0)
        {
            pages[pageIndex].gameObject.SetActive(true);
            pauseTab.SetActive(false);
            pages[pageIndex].updateState();
        }
        else
        {
            pauseTab.SetActive(true);
        }
    }
}
