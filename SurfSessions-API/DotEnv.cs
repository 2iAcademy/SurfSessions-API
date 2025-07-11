﻿using System.Text.RegularExpressions;

namespace SurfSessions_API;

using System;
using System.IO;

public static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine(".env file named \"" + filePath + "\" not found");
            return;
        }

        foreach (var line in File.ReadAllLines(filePath))
        {
            // Ne charge pas les lignes commentées
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;
            
            int index = line.IndexOf('=');
            
            // Ne charge pas les lignes invalides
            if (index == -1)
                continue;
            
            // Sépare uniquement au premier "="
            var key = line.Substring(0, index).Trim();
            var value = line.Substring(index + 1).Trim();
            
            // Retire les "" et ''
            if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                (value.StartsWith("'") && value.EndsWith("'")))
            {
                value = value.Substring(1, value.Length - 2);
            }
            
            // Remplace les variables imbriquées ${VAR} par leur valeur 
            value = Regex.Replace(value, @"\$\{([^}]+)\}", match =>
            {
                string varName = match.Groups[1].Value;
                return Environment.GetEnvironmentVariable(varName) ?? "";
            });

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}