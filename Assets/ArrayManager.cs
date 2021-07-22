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

    static public Tuple<int, int> GetBoundsFromPercentRange(int[] input, float lowerBoundPercent, float higherBoundPercent)
    {
        Array.Sort(input);

        int min = input[(int)((input.Length-1) * lowerBoundPercent)];
        int max = input[(int)((input.Length-1) * higherBoundPercent)];

        Debug.Log("Median: "+input[(int)((input.Length-1) * 0.5f)]+", Min: "+min+", Max: "+max);

        return new Tuple<int, int>(min, max);
    }

    static public int[] IntValueCounter(int[] inputArray, int values)
    {
        int[] array = new int[values];

        for(int i = 0; i < inputArray.Length; i++) {
            array[inputArray[i]]++;
        }

        return array;
    }

    static public int GetHighestIndex(int[] array)
    {
        int highestValue = 0;
        int highestIndex = 0;

        for(int i = 0; i < array.Length; i++) {
            int count = 0;
            for(int j = 0; j < 11; j++) {
                count += array[KeepInCircularRange(0, array.Length-1, i-j+5)];
            }
            if(count > highestValue) {
                highestValue = count;
                highestIndex = i;
            }
        }

        return highestIndex;
    }

    static public Tuple<int, int> GetBoundsOfHighestDensityValues(int[] array, int highestIndex, float sensitivity)
    {
        int[] bounds = new int[2];

        for(int i = 0; i < 2; i++) {
            int iterations = 0;
            int lowest = array.Max();

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

                if(array[indexValue] > array[bounds[i]] * sensitivity) break;
            }
        }

        Debug.Log("Max Index: "+highestIndex+", Min: "+bounds[0]+", Max: "+bounds[1]);

        return new Tuple<int, int>(bounds[0], bounds[1]);
    }
}
