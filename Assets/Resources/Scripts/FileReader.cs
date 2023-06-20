using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileReader : MonoBehaviour
{
    public Dictionary<string, string[]> readFile(string fileName)
    {
        Dictionary<string, string[]> recipes = new Dictionary<string, string[]>();
        string filePath = "Assets/Resources/Files/" + fileName + ".txt";

        StreamReader file = new StreamReader(filePath);
        string[] lines = new string[File.ReadAllLines(filePath).Length];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = file.ReadLine();
        }
        file.Close();

        foreach (string line in lines)
        {
            recipes.Add(line.Split(": ")[0], line.Split(": ")[1].Split("; "));
        }

        return recipes;
    }
}
