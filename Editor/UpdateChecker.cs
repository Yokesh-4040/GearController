using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[InitializeOnLoad]
public class UpdateChecker
{
    static UpdateChecker()
    {
        // This code will be executed when the assembly is loaded (i.e., when Unity opens)
        CheckForUpdates();
    }

    private static void CheckForUpdates()
    {
        string packageName = "GearController";
        string currentVersion = "1.0.0"; // Current version of your package

        string repoPath = "Packages/GearController"; // Path to your Unity package within the project

        string gitCommand = FindGitExecutable();
        if (string.IsNullOrEmpty(gitCommand))
        {
            Debug.LogError("Git executable not found. Make sure Git is installed and available in your system's PATH.");
            return;
        }

        string arguments = "ls-remote --tags origin";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = gitCommand,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Application.dataPath + "/../" + repoPath // Adjust the path to the "Packages" directory
        };

        using (Process process = new Process { StartInfo = psi })
        {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (string.IsNullOrEmpty(error))
            {
                string[] tagLines = output.Split('\n');
                if (tagLines.Length > 0)
                {
                    string latestTag = tagLines[0].Split('\t')[1].Trim();
                    if (IsNewVersionAvailable(latestTag, currentVersion))
                    {
                        Debug.Log($"A new version ({latestTag}) of {packageName} is available!");
                        // Implement your update logic here
                    }
                    else
                    {
                        Debug.Log($"You have the latest version of {packageName}.");
                    }
                }
                else
                {
                    Debug.LogWarning("No tags found. Consider adding tags to your GitHub repository for versioning.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to check for updates: {error}");
            }
        }
    }

    private static string FindGitExecutable()
    {
        // Try to find the Git executable in the system's PATH
        string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
        foreach (string path in paths)
        {
            string gitPath = Path.Combine(path, "git.exe");
            if (File.Exists(gitPath))
            {
                return gitPath;
            }
        }

        return null;
    }

    private static bool IsNewVersionAvailable(string latestTag, string currentVersion)
    {
        Version latest = new Version(latestTag.TrimStart("refs/tags/v".ToCharArray()));
        Version current = new Version(currentVersion);

        return latest > current;
    }
}
