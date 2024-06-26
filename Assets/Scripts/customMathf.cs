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
}
