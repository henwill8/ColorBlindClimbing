using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ArrayManager : MonoBehaviour
{  
    static public int KeepInCircularRange(int min, int max, int value)
    {
        while(value > max || value < min) {
            if(value > max) value -= max - min + 1;
            if(value < min) value += max - min + 1;
        }
        return value;
    }

    static public int[] RemoveOutOfBoundValues(int[] input, int[] reference, int min, int max)
    {
        int[] array = new int[input.Length];
        int skipped = 0;
        
        for(int i = 0; i < input.Length; i++) {
            if((min <= max && (reference[i] >= min && reference[i] <= max)) || (min >= max && (reference[i] >= min || reference[i] <= max))) {
                array[i - skipped] = input[i];
            } else {
                skipped++;
            }
        }
        
        Array.Resize(ref array, array.Length - skipped);

        return array;
    }

    static public int[] IntValueCounter(int[] inputArray, int values)
    {
        int[] array = new int[values];

        for(int i = 0; i < inputArray.Length; i++) {
            array[inputArray[i]]++;
        }

        return array;
    }

    static public int GetAverageValue(int[] array, int index, int range)
    {
        int totalValues = range * 2 + 1;
        int count = 0;
        
        for(int i = 0; i < totalValues; i++) {
            count += array[KeepInCircularRange(0, array.Length-1, index + i + range)];
        }

        return (int)(count / totalValues);
    }

    static public int[] ExtremifyArray(int[] array, float extremity)
    {
        int max = array.Max();
        int min = array.Min();
        int midpoint = (max + min)/2;
        int quaterpoint = (midpoint + min)/2;

        // Debug.Log(max+" "+min+" "+midpoint);

        for(int i = 0; i < array.Length; i++) {
            if(array[i] > midpoint) {
                array[i] = (int)Mathf.Lerp(array[i], max, extremity);
            } else {
                array[i] = 0;
            }
        }

        return array;
    }

    static public int[] NormalizeArray(int[] array, int maxValue)
    {
        int max = array.Max();
        
        for(int i = 0; i < array.Length; i++) {
            array[i] = (int)(((float)array[i] / (float)max) * maxValue);
        }

        return array;
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
        for(int i = 0; i < array.Length; i++) {
            if(!Utils.IsInCircularRange(i, minIndex, maxIndex)) {
                array[i] = 0;
            }
        }

        return array;
    }

    static public int[] SmoothIntArray(int[] array, int range)
    {
        int[] smoothedArray = new int[array.Length];

        for(int i = 0; i < smoothedArray.Length; i++) {
            smoothedArray[i] = GetAverageValue(array, i, range);
        }

        return smoothedArray;
    }

    static public int GetHighestAverageIndex(int[] array, int distance)
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

    static public Tuple<int, int> GetBoundsOfHighestDensityValues(int[] array, float sensitivity, int highestIndex = -1)
    {
        if(highestIndex == -1) highestIndex = ArrayManager.GetHighestAverageIndex(array, 0);
        int[] bounds = new int[2];

        for(int i = 0; i < 2; i++) {
            int iterations = 0;
            int lowest = array.Max();

            int flatIndex = highestIndex;
            float flatSensitivity = 1.5f;

            bounds[i] = highestIndex;

            while(iterations > -100 && iterations < 100) {
                if(i == 0) {
                    iterations--;
                } else {
                    iterations++;
                }

                int indexValue = KeepInCircularRange(0, array.Length-1, highestIndex + iterations);

                if(array[indexValue] < lowest || lowest == array.Max()) {
                    bounds[i] = indexValue;
                    lowest = array[indexValue];
                }
                if(array[indexValue] > array[flatIndex] * flatSensitivity || array[indexValue] < array[flatIndex] * (1 / flatSensitivity)) {
                    flatIndex = indexValue;
                }
                
                if(System.Math.Abs(flatIndex - indexValue) > 20 && System.Math.Abs(highestIndex - indexValue) > 10) {
                    bounds[i] = flatIndex;
                    Debug.Log("Hue is flat");
                    break;
                }
                if(array[indexValue] > array[bounds[i]] * sensitivity) {
                    Debug.Log("Hue is too large");
                    break;
                }
            }
        }

        Debug.Log("Min: "+bounds[0]+", Max Index: "+highestIndex+", Max: "+bounds[1]);

        return new Tuple<int, int>(bounds[0], bounds[1]);
    }
}
