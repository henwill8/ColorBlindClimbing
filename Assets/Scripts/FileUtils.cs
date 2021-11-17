using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileUtils
{
    static public void IntArrayToFile(int[] array, string filePath) {
        Debug.Log("Writing Int Array to "+Path.Combine(Application.persistentDataPath, filePath));
        using (BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(Application.persistentDataPath, filePath), FileMode.Create)))
        {
            byte[] bytes = new byte[array.Length * sizeof(int)];
            Buffer.BlockCopy(array, 0, bytes, 0, bytes.Length);

            writer.Write(bytes);
        }
    }

    static public int[] ReadFileToIntArray(string filePath) {
        Debug.Log("Reading Int Array from "+Path.Combine(Application.persistentDataPath, filePath));
        using (BinaryReader reader = new BinaryReader(File.Open(Path.Combine(Application.persistentDataPath, filePath), FileMode.Open)))
        {
            int intLength = (int)reader.BaseStream.Length / sizeof(int);
            int[] intArray = new int[intLength];

            for(int i = 0; i < intLength; i++) {
                intArray[i] = reader.ReadInt32();
            }//There must be a better way to do this but oh well
            
            return intArray;
        }
    }
}
