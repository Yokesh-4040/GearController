using UnityEditor;
using UnityEngine;

namespace FourtyFourty.GearController.Editor
{
    [InitializeOnLoad]
    public class GearControllerManager : MonoBehaviour
    {
        private const string PlayerPrefKey = "YourPlayerPrefKey";
        private static bool _isPopUpShown;

        static GearControllerManager()
        {
            // This code will be executed when Unity loads

            if (PlayerPrefs.HasKey(PlayerPrefKey) || _isPopUpShown) return;
            ShowPopup();
            _isPopUpShown = true;
        }
    
        [MenuItem("Window/Show License Expiry Popup")]
        private static void ShowLicenseExpiryPopup()
        {
            if (PlayerPrefs.HasKey(PlayerPrefKey) || _isPopUpShown) return;
            ShowPopup();
            _isPopUpShown = true;
        }

        private static void ShowPopup()
        {
            string message = "This project license is Expired. Please contact the developer";
            string title = "License Expiry";

            if (EditorUtility.DisplayDialog(title, message, "Close Tab", "Quit Unity Editor"))
            {
                // User clicked "Close Tab"
                // Perform actions if needed
            }
            else
            {
                // User clicked "Quit Unity Editor"
                EditorApplication.ExitPlaymode();
                EditorApplication.isPlaying = false;
                AssetDatabase.SaveAssets();
                EditorApplication.Exit(0);
            }
        }
    
        private void OnLostFocus()
        {
            // This method is called when the window loses focus, such as when the close (X) button is clicked
            EditorApplication.ExitPlaymode();
            EditorApplication.isPlaying = false;
            AssetDatabase.SaveAssets();
            EditorApplication.Exit(0);
        }
    }
}