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
    }//Not using

    static public int[] IntValueCounter(int[] array, int values)
    {
        int[] newArray = new int[values];

        for(int i = 0; i < array.Length; i++) {
            newArray[array[i]]++;
        }

        return newArray;
    }

    static public int[] ExtremifyArray(int[] array)
    {
        int[] newArray = new int[array.Length];
        int max = array.Max();
        int min = array.Min();
        int difference = max - min;

        // Debug.Log(max+" "+min+" "+midpoint);

        for(int i = 0; i < array.Length; i++) {
            newArray[i] = (int)(difference * Utils.EaseInOutExpo((float)array[i] / (float)difference));
            if(newArray[i] < (difference * 0.25f) + min) newArray[i] = 0;
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

    static public int[] MergeArrays(int[] arrayA, int[] arrayB, int aBias = 1, int bBias = 1, int filteringStrength = 10)
    {
        int maxLength = System.Math.Max(arrayA.Length, arrayB.Length);
        int[] mergedArray = new int[maxLength];

        int maxValue = System.Math.Max(arrayA.Max(), arrayB.Max());

        for(int i = 0; i < maxLength; i++) {
            if(arrayB[i] < (float)maxValue / (float)filteringStrength) {
                mergedArray[i] = arrayA[i];
            } else if(arrayA[i] < (float)maxValue / (float)filteringStrength) {
                mergedArray[i] = arrayB[i];
            } else {
                mergedArray[i] = (arrayA[i] * aBias + arrayB[i] * bBias) / (aBias+bBias);
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

    static public int[] SmoothIntArray(int[] array, int range, bool keepCircular = true)
    {
        int[] smoothedArray = new int[array.Length];

        for(int i = 0; i < smoothedArray.Length; i++) {
            smoothedArray[i] = GetAverageValue(array, i, range, keepCircular);
        }

        return smoothedArray;
    }

    static public int GetAverageValue(int[] array, int index, int range, bool keepCircular = true)
    {
        if(range == 0) return array[index];

        int totalValues = range * 2 + 1;
        int skippedValues = 0;
        int count = 0;
        
        for(int i = 0; i < totalValues; i++) {
            int rawIndex = index + i - (range);
            int circularIndex = Utils.KeepInCircularRange(rawIndex, 0, array.Length-1);

            if(!keepCircular && (rawIndex < 0 || rawIndex > array.Length-1)) {
                skippedValues++;
                continue;
            }

            count += array[circularIndex];
        }

        return (int)(count / (totalValues - skippedValues));
    }//Check if this works at the array ends when not circular

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

    static public Tuple<int, int> GetBoundsOfHighestDensityValues(int[] array, float sensitivity, int flatDistance = 10, int flatSensitivity = 10, bool keepFlatEndpoint = false, int flatBias = 2, bool keepCircular = true, int highestIndex = -1, int maxIterations = 100)
    {
        if(highestIndex == -1) highestIndex = ArrayManager.GetHighestAverageIndex(array, 0);
        int[] bounds = new int[2];

        for(int i = 0; i < 2; i++) {
            int iterations = 0;
            int lowest = array.Max();

            int flatBounds = array.Max() / flatSensitivity;
            int flatIndex = highestIndex;
            int flatIteration = 0;

            bounds[i] = highestIndex;

            while(iterations > -maxIterations && iterations < maxIterations) {
                if(i == 0) {
                    iterations--;
                } else {
                    iterations++;
                }

                int rawIndex = highestIndex + iterations;
                int indexValue = Utils.KeepInCircularRange(rawIndex, 0, array.Length-1);
                if(!keepCircular && (rawIndex <= 0 || rawIndex >= array.Length-1)) {
                    Debug.Log("Value has reached edge of array");
                    bounds[i] = rawIndex;
                    break;
                }

                if(array[indexValue] < lowest || lowest == array.Max()) {
                    bounds[i] = indexValue;
                    lowest = array[indexValue];
                } else if(array[indexValue] > lowest) {
                    Debug.Log("Value is greater or equal to current lowest "+bounds[i]+" "+indexValue);
                }
                if(array[indexValue] > array[flatIndex] + flatBounds || array[indexValue] < array[flatIndex] - flatBounds) {
                    flatIndex = indexValue;
                    flatIteration = System.Math.Abs(iterations);
                }
                
                int flatEndValue = indexValue;
                if(!keepFlatEndpoint) flatEndValue = flatIndex;

                if(System.Math.Abs(iterations) - flatIteration > flatDistance && System.Math.Abs(iterations) > flatDistance && array[flatEndValue] < array.Max() / flatBias) {
                    bounds[i] = flatEndValue;
                    Debug.Log("Value is flat");
                    break;
                }
                if(array[indexValue] > array[bounds[i]] * sensitivity) {//This seems to be finding the top of the nearest mountain better than the function I specifically wrote for it does?
                    Debug.Log("Value is too large");
                    break;
                }
            }
        }

        Debug.Log("Min: "+bounds[0]+", Max Index: "+highestIndex+", Max: "+bounds[1]);

        return new Tuple<int, int>(bounds[0], bounds[1]);
    }
}
