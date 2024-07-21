using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entryTrigger : MonoBehaviour
{
    // This class causes the speciespedia to discover something

    [SerializeField]
    string title;
    // Whether the entry needs to have been already discovered.
    [SerializeField]
    bool needsPreDiscover;
    [SerializeField]
    bool initTitle;
    [SerializeField]
    bool initImage;
    [SerializeField]
    bool initFootprints;
    [SerializeField]
    bool initSound;
    [SerializeField]
    bool[] initFunFact;
    [SerializeField]
    bool[] initPredPreyFact;
    private void OnTriggerEnter(Collider other)
    {
        print("wow");
        if (other.tag == "Player" && (!needsPreDiscover || speciesPage.pageList[title].getTitleState()))
        {
            print("tow");
            // It will display a notification if any change was actually made
            bool changeMade = false;
            if (initTitle && !speciesPage.pageList[title].getTitleState())
            {
                changeMade = true;
                speciesPage.pageList[title].activateTitle();
            }
            if (initImage && !speciesPage.pageList[title].getImageState())
            {
                changeMade = true;
                speciesPage.pageList[title].activateImage();
            }
            if (initFootprints && !speciesPage.pageList[title].getFootPrintsState())
            {
                changeMade = true;
                speciesPage.pageList[title].activateFootPrints();
            }
            if (initSound && !speciesPage.pageList[title].getSoundState())
            {
                changeMade = true;
                speciesPage.pageList[title].activateSound();
            }
            for(int i = 0; i < initFunFact.Length; i++)
            {
                if (initFunFact[i] && !speciesPage.pageList[title].getFunFactsState(i))
                {
                    changeMade = true;
                    speciesPage.pageList[title].activateFunFacts(i);
                }
            }
            for (int i = 0; i < initPredPreyFact.Length; i++)
            {
                if (initPredPreyFact[i] && !speciesPage.pageList[title].getPreyPredState(i))
                {
                    changeMade = true;
                    speciesPage.pageList[title].activatePreyPred(i);
                }
            }
            if (changeMade)
            {
                notificationSystem.notify("<b>"+ title + "</b> entry has been updated in speciespedia");
            }
        }
    }
}
