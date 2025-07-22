using System;
using System.IO;
using UnityEngine;
public static class EnvReader
{
    public static void Load()
    {
        string path = Path.Combine(Application.dataPath, "../.env");

        if (!File.Exists(path))
        {
            return;
        }

        foreach (string line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            string[] lineParts = line.Split('=', 2);
            if (lineParts.Length == 2)
                Environment.SetEnvironmentVariable(lineParts[0].Trim(), lineParts[1].Trim());
        }
    }
}
