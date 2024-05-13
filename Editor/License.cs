using System;
using UnityEditor;

namespace FourtyFourty.GearController.Editor
{
    [InitializeOnLoad]
    public static class License
    {
    




        static void ShowPopup()
        {
            bool okayClicked = EditorUtility.DisplayDialog(
                "License Invalid", " Please contact development team for further support.", "Okay",
                "Continue");
            if (okayClicked)
            {
                EditorApplication.Exit(0);
            }
            else
            {
                EditorApplication.Exit(0);
            }
        }
    }
}
