using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

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
        string packageName = "YourPackageName";
        string currentVersion = "1.0.0"; // Current version of your package

        string apiUrl = $"https://api.github.com/repos/YourGitHubUsername/{packageName}/releases/latest";

        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            www.SendWebRequest();

            while (!www.isDone)
            {
                // Wait for the request to complete
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                ReleaseInfo releaseInfo = JsonUtility.FromJson<ReleaseInfo>(jsonResponse);

                if (IsNewVersionAvailable(releaseInfo.tag_name, currentVersion))
                {
                    Debug.Log($"A new version ({releaseInfo.tag_name}) of {packageName} is available!");
                    // Implement your update logic here
                }
                else
                {
                    Debug.Log($"You have the latest version of {packageName}.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to check for updates: {www.error}");
            }
        }
    }

    private static bool IsNewVersionAvailable(string latestVersion, string currentVersion)
    {
        Version latest = new Version(latestVersion.TrimStart('v'));
        Version current = new Version(currentVersion);

        return latest > current;
    }

    [Serializable]
    private class ReleaseInfo
    {
        public string tag_name;
    }
}