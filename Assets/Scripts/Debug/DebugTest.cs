using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{ 
    // This class houses the debug testing that can happen within a simple print function
    // Start is called before the first frame update
    void Start()
    {
        // Debugginf container list
        void printContainerList(customMathf.SortContainer<string>[] containerList)
        {
            string retString = "";
            for (int i = 0; i < containerList.Length; i++)
            {
                retString += containerList[i].contained + "|" + containerList[i].value;
                retString += " ,";
            }
            print(retString);
        }
        customMathf.SortContainer<string>[] testContainer = new customMathf.SortContainer<string>[5];
        testContainer[0] = new customMathf.SortContainer<string>("wow", 0);
        testContainer[1] = new customMathf.SortContainer<string>("wow", -2);
        testContainer[2] = new customMathf.SortContainer<string>("wow", 6);
        testContainer[3] = new customMathf.SortContainer<string>("wow", (float)23.6);
        testContainer[4] = new customMathf.SortContainer<string>("wow", 23);
        printContainerList(testContainer);
        customMathf.mergeSort<string>(testContainer);
        printContainerList(testContainer);
        testContainer = new customMathf.SortContainer<string>[1];
        testContainer[0] = new customMathf.SortContainer<string>("wow", 0);
        printContainerList(testContainer);
        customMathf.mergeSort<string>(testContainer);
        printContainerList(testContainer);
        testContainer = new customMathf.SortContainer<string>[10];
        testContainer[0] = new customMathf.SortContainer<string>("wow", 0);
        testContainer[1] = new customMathf.SortContainer<string>("wow", -2);
        testContainer[2] = new customMathf.SortContainer<string>("wow", 6);
        testContainer[3] = new customMathf.SortContainer<string>("wow", (float)23.6);
        testContainer[4] = new customMathf.SortContainer<string>("wow", 4);
        testContainer[5] = new customMathf.SortContainer<string>("wow", 05);
        testContainer[6] = new customMathf.SortContainer<string>("wow", -24);
        testContainer[7] = new customMathf.SortContainer<string>("wow", 4);
        testContainer[8] = new customMathf.SortContainer<string>("wow", (float)23.6);
        testContainer[9] = new customMathf.SortContainer<string>("wow", 7);
        printContainerList(testContainer);
        customMathf.mergeSort<string>(testContainer);
        printContainerList(testContainer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
