using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customMathf : MonoBehaviour
{
    #region randomization
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
    #endregion
    #region list functions
    public static bool contains<T>(T[] array, T value)
    {
        for(int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(value))
            {
                return true;
            }
        }
        return false;
    }
    #endregion
    #region vector math
    // Finds distance between two points
    public static float distanceBetweenPoints(Vector3 point1, Vector3 point2)
    {
        Vector3 diff = point1 - point2;
        return diff.magnitude;
    }
    #endregion
    #region sorting
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
        public SortContainer(float setValue)
        {
            value = setValue;
        }
    }

    // Merge sort, modifies inputted container list
    // Goes from least to greatest
    public static void mergeSort<T>(SortContainer<T>[] containerList)
    {
        SortContainer<T>[] tempList = new SortContainer<T>[containerList.Length];
        for(int width = 1; width < containerList.Length * 2; width = 2 * width)
        {
            for(int i = 0; i < containerList.Length; i += width * 2)
            {
                merge<T>(containerList, i, width, tempList);
            }
            importContainerList(containerList, tempList);
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
            orgRight = containerList.Length;
        }
        int tempRight = orgRight;
        if(orgEnd > containerList.Length)
        {
            orgEnd = containerList.Length;
        }
        for (int i = orgLeft; i < orgEnd; i++)
        {
            if(tempLeft < orgRight && (tempRight >= orgEnd || containerList[tempLeft].value <= containerList[tempRight].value))
            {
                tempList[i] = containerList[tempLeft];
                tempLeft += 1;
            }
            else
            {
                tempList[i] = containerList[tempRight];
                tempRight += 1;
            }
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
    #endregion
}
