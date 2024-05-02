using System;
using UnityEditor;

namespace FourtyFourty.GearController.Editor
{
    [InitializeOnLoad]
    public static class License
    {
        private static DateTime _expirationDate = new DateTime(2024, 5, 5);
        private const int ExpirationHour = 18;
        private const int ExpirationMinute = 0;

        static License()
        {
            CheckExpiry();
        }


        static void CheckExpiry()
        {
            DateTime expiryDateTime = new DateTime(_expirationDate.Year, _expirationDate.Month, _expirationDate.Day,
                ExpirationHour, ExpirationMinute, 0);
            TimeSpan remainingTime = expiryDateTime - DateTime.Now;
            if (remainingTime.TotalSeconds < 0)
            {
               ShowPopup();
               EditorApplication.Exit(0);
            }
            else
            {
                //Debug.Log("Not expired yet. Remaining time: " + remainingTime);
            }
        }

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
