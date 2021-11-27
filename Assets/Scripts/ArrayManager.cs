using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ArrayManager : MonoBehaviour
{
    static public int FindTopOfNearestHill(int[] array, int index)
    {
        // int checkDistance = 1;
        // // int lastValueLeft = array[index];
        // int lastValueRight = array[index];

        // while(checkDistance < 50) {//Make sure to add rounded index to make it not die
        //     Debug.Log(checkDistance + " "+index);
        //     // if(array[index-checkDistance] > lastValueLeft) {
        //     //     lastValueLeft = array[index-checkDistance];
        //     // Debug.Log(lastValueLeft);
        //     // } else if(array[index-checkDistance] > array[index]*1.25f) {
        //     //     return index-checkDistance;
        //     // }
        //     Debug.Log(array[index+checkDistance]+" "+lastValueRight);
        //     if(array[index+checkDistance] > lastValueRight) {
        //         lastValueRight = array[index+checkDistance];
        //     } else if(array[index+checkDistance] > array[index]*1.25f) {
        //         return index+checkDistance;
        //     }

        //     checkDistance++;
        // }
        // Debug.Log("BADDDD");
        return index;
    }//Likely get rid of this

    static public int[] RemoveOutOfBoundValues(int[] array, int[] reference, int min, int max)
    {
        int[] newArray = new int[array.Length];
        int skipped = 0;
        
        for(int i = 0; i < array.Length; i++) {
            if((min <= max && (reference[i] >= min && reference[i] <= max)) || (min >= max && (reference[i] >= min || reference[i] <= max))) {
                newArray[i - skipped] = array[i];
            } else {
                skipped++;
            }
        }
        
        Array.Resize(ref newArray, newArray.Length - skipped);

        return newArray;
    }

    static public int[] IntValueCounter(int[] array, int values)
    {
        int[] newArray = new int[values];

        for(int i = 0; i < array.Length; i++) {
            newArray[array[i]]++;
        }

        return newArray;
    }

    static public int[] ExtremifyArray(int[] array, float extremity)
    {
        int[] newArray = new int[array.Length];
        int max = array.Max();
        int min = array.Min();
        int midpoint = (max + min)/2;
        int quaterpoint = (midpoint + min)/2;

        // Debug.Log(max+" "+min+" "+midpoint);

        for(int i = 0; i < array.Length; i++) {
            if(array[i] > quaterpoint) {
                newArray[i] = (int)Mathf.Lerp(array[i], max, extremity);
            } else {
                newArray[i] = 0;
            }
        }

        return newArray;
    }

    static public int[] NormalizeArray(int[] array, int maxValue)
    {
        int[] newArray = new int[array.Length];
        int max = array.Max();
        
        for(int i = 0; i < array.Length; i++) {
            newArray[i] = (int)(((float)array[i] / (float)max) * maxValue);
        }

        return newArray;
    }

    static public int[] MergeArrays(int[] arrayA, int[] arrayB)
    {
        int maxLength = System.Math.Max(arrayA.Length, arrayB.Length);
        int[] mergedArray = new int[maxLength];

        for(int i = 0; i < maxLength; i++) {
            if(arrayB[i] == 0) {
                mergedArray[i] = arrayA[i];
            } else if(arrayA[i] == 0) {
                mergedArray[i] = arrayB[i];
            } else {
                mergedArray[i] = (arrayA[i]*2 + arrayB[i]) / 3;
            }
        }

        return mergedArray;
    }

    static public int[] AddArrays(int[] arrayA, int[] arrayB)
    {
        int maxLength = System.Math.Max(arrayA.Length, arrayB.Length);
        int[] mergedArray = new int[maxLength];

        for(int i = 0; i < maxLength; i++) {
            mergedArray[i] = arrayA[i] + arrayB[i];
        }

        return mergedArray;
    }

    static public int[] RemoveValuesOutOfIndexRange(int[] array, int minIndex, int maxIndex)
    {
        int[] newArray = new int[array.Length];

        for(int i = 0; i < array.Length; i++) {
            if(!Utils.IsInCircularRange(i, minIndex, maxIndex)) {
                newArray[i] = 0;
            } else {
                newArray[i] = array[i];
            }
        }

        return newArray;
    }

    static public int[] SmoothIntArray(int[] array, int range)
    {
        int[] smoothedArray = new int[array.Length];

        for(int i = 0; i < smoothedArray.Length; i++) {
            smoothedArray[i] = GetAverageValue(array, i, range);
        }

        return smoothedArray;
    }

    static public int GetAverageValue(int[] array, int index, int range)
    {
        if(range == 0) return array[index];

        int totalValues = range * 2 + 1;
        int count = 0;
        
        for(int i = 0; i < totalValues; i++) {
            count += array[Utils.KeepInCircularRange(0, array.Length-1, index + i + range)];
        }

        return (int)(count / totalValues);
    }

    static public int GetHighestAverageIndex(int[] array, int distance = 0)
    {
        int highestValue = 0;
        int highestIndex = 0;

        for(int i = 0; i < array.Length; i++) {
            int average = GetAverageValue(array, i, distance);

            if(average > highestValue) {
                highestValue = average;
                highestIndex = i;
            }
        }

        return highestIndex;
    }

    static public Tuple<int, int> GetBoundsOfHighestDensityValues(int[] array, float sensitivity, int flatDistance = 10, float flatSensitivity = 1.5f, bool keepCircular = true, int highestIndex = -1, int maxIterations = 100)
    {
        if(highestIndex == -1) highestIndex = ArrayManager.GetHighestAverageIndex(array, 0);
        int[] bounds = new int[2];

        for(int i = 0; i < 2; i++) {
            int iterations = 0;
            int lowest = array.Max();

            int flatIndex = highestIndex;

            bounds[i] = highestIndex;

            while(iterations > -maxIterations && iterations < maxIterations) {
                if(i == 0) {
                    iterations--;
                } else {
                    iterations++;
                }

                int indexValue = Utils.KeepInCircularRange(0, array.Length-1, highestIndex + iterations);
                if(!keepCircular && (highestIndex + iterations <= 0 || highestIndex + iterations >= array.Length-1)) {
                    bounds[i] = indexValue;
                    break;
                }

                if(array[indexValue] < lowest || lowest == array.Max()) {
                    bounds[i] = indexValue;
                    lowest = array[indexValue];
                }
                if(array[indexValue] > array[flatIndex] * flatSensitivity || array[indexValue] < array[flatIndex] * (1.0f / flatSensitivity)) {
                    flatIndex = indexValue;
                }
                
                if(System.Math.Abs(flatIndex - indexValue) > flatDistance && System.Math.Abs(highestIndex - indexValue) > flatDistance) {
                    // bounds[i] = flatIndex;
                    Debug.Log("Hue is flat");
                    break;
                }
                if(array[indexValue] > array[bounds[i]] * sensitivity) {//This seems to be finding the top of the nearest mountain better than the function I specifically wrote for it does?
                    Debug.Log("Hue is too large");
                    break;
                }
            }
        }

        Debug.Log("Min: "+bounds[0]+", Max Index: "+highestIndex+", Max: "+bounds[1]);

        return new Tuple<int, int>(bounds[0], bounds[1]);
    }
}
