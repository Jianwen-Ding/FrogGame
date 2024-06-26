using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customMathf : MonoBehaviour
{
    // This class covers custom math functions that might be useful
    public static int weightedRandomRange(float[] weights)
    {
        float rangeTotal = 0;
        for(int i = 0; i < weights.Length; i++)
        {
            rangeTotal += weights[i];
        }
        float randomGenerate = Random.Range(0, rangeTotal);
        float rangeFloor = 0;
        for(int i = 0; i < weights.Length; i++)
        {
            rangeFloor = rangeFloor + weights[i];
            if(randomGenerate < rangeFloor)
            {
                return i;
            }
        }
        return weights.Length - 1;
    }
    // For sorting
    // (Partially this entire thing is for me to practice doing sorts lol)
    public class SortContainer<T>
    {
        public T contained;
        public float value;
        public SortContainer(T setContainer, float setValue)
        {
            contained = setContainer;
            value = setValue;
        }
    }

    // Merge sort, modifies inputted container list
    public static void mergeSort<T>(SortContainer<T>[] containerList)
    {
        SortContainer<T>[] tempList = new SortContainer<T>[containerList.Length];
        for(int width = 1; width < containerList.Length * 2; width = 2 * width)
        {
            for(int i = 0; i < containerList.Length; i += width * 2)
            {
                
            }
        }

    }
    // Component of merge sort, actually merges together the containers
    public static void merge<T>(SortContainer<T>[] containerList, int startOfIndex, int width, SortContainer<T>[] tempList)
    {
        int orgLeft = startOfIndex;
        int tempLeft = orgLeft;
        int orgRight = startOfIndex + width;
        int orgEnd = startOfIndex + 2 * width;
        if(orgRight > containerList.Length)
        {
            orgRight = containerList.Length - 1;
        }
        int tempRight = orgRight;
        if(orgEnd > containerList.Length)
        {

        }
    }
    // Copies a list over one list
    public static void importContainerList<T>(SortContainer<T>[] importedToList, SortContainer<T>[] exportingList)
    {
        for(int i = 0; i < importedToList.Length; i++)
        {
            importedToList[i] = exportingList[i];
        }
    }
}
