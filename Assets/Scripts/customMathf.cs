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
    // Finds angle between two points
    public static float angleBetweenTwoVecs(Vector3 vec1, Vector3 vec2)
    {
        float dotProd = vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
        float cosAng = dotProd / ((Mathf.Abs(vec1.magnitude) * Mathf.Abs(vec2.magnitude)));
        return Mathf.Acos(cosAng) * Mathf.Rad2Deg;
    }
    // Finds x,z vector using angle
    // Uses degrees
    public static Vector3 angleToPoint(float angle, float magnitude)
    {
        float radAngle = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radAngle) * magnitude;
        float z = Mathf.Sin(radAngle) * magnitude;
        return new Vector3(x, 0, z);
    }
    // angle using x and z
    // x is adj
    // z is opp
    // Uses degrees
    public static float pointToAngle(float adj, float opp)
    {
        return Mathf.Atan2(opp,adj) * Mathf.Rad2Deg;
    }
    #endregion
    #region sorting
    // (Partially this entire thing is for me to practice doing sorts lol)

    // To Convert objects into values to be sorted
    public delegate float valueFinder<T>(T objectSorting);

    // To Store objects with their values
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
    // >>> MERGE SORT <<<
    // Uses sort container
    // Container for values to be sorted

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

    // Merge sort using delegates
    // Goes from least to greatest
    public static void mergeSort<T>(T[] containerList, valueFinder<T> valueFinder)
    {
        T[] tempList = new T[containerList.Length];
        for (int width = 1; width < containerList.Length * 2; width = 2 * width)
        {
            for (int i = 0; i < containerList.Length; i += width * 2)
            {
                merge<T>(containerList, i, width, tempList, valueFinder);
            }
            importContainerList(containerList, tempList);
        }

    }
    // Component of merge sort, actually merges together the containers
    public static void merge<T>(T[] containerList, int startOfIndex, int width, T[] tempList, valueFinder<T> valueFinder)
    {
        int orgLeft = startOfIndex;
        int tempLeft = orgLeft;
        int orgRight = startOfIndex + width;
        int orgEnd = startOfIndex + 2 * width;
        if (orgRight > containerList.Length)
        {
            orgRight = containerList.Length;
        }
        int tempRight = orgRight;
        if (orgEnd > containerList.Length)
        {
            orgEnd = containerList.Length;
        }
        for (int i = orgLeft; i < orgEnd; i++)
        {
            if (tempLeft < orgRight && (tempRight >= orgEnd || valueFinder(containerList[tempLeft]) <= valueFinder(containerList[tempRight])))
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
    public static void importContainerList<T>(T[] importedToList, T[] exportingList)
    {
        for (int i = 0; i < importedToList.Length; i++)
        {
            importedToList[i] = exportingList[i];
        }
    }


    // >>> FIND GREATEST <<<
    // Returns the object in the list with the greatest value
    // Assumes list is not empty
    public static T findGreatest<T>(T[] list, valueFinder<T> valueUsed)
    {
        if(list.Length == 0)
        {
            print("ERROR- Nothing in list to find");
            return list[0];
        }
        else
        {
            T greatest = list[0];
            float greatestValue = valueUsed(list[0]);
            for(int i = 1; i < list.Length; i++)
            {
                float getVal = valueUsed(list[i]);
                if (greatestValue < getVal)
                {
                    greatest = list[i];
                    greatestValue = getVal;
                }
            }
            return greatest;
        }
    }
    // Placeholder is what returns if dictionary is empty
    public static T findGreatestKeys<T,Z>(Dictionary<T,Z> dict, valueFinder<T> valueUsed, T placeholder)
    {
        if (dict.Count == 0)
        {
            print("ERROR- Nothing in list to find");
        }
        bool hasStarted = false;
        T greatest = placeholder;
        // Could cause error if value is somehow lesser
        float greatestValue = -9999999;
        foreach(T ob in dict.Keys)
        {
            float getVal = valueUsed(ob);
            if (!hasStarted)
            {
                if (greatestValue < getVal)
                {
                    greatest = ob;
                    greatestValue = getVal;
                }
            }
            else
            {
                greatestValue = getVal;
                greatest = ob;
            }
        }
        return greatest;
    }
    #endregion
    #region Containers
    public struct duo<T, Z>
    {
        T val1;
        Z val2;
    }
    #endregion
}
