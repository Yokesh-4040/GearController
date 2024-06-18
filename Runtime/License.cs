using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class License 
{
    private static readonly DateTime ExpirationDate = new DateTime(2024, 5, 30);
    private const int ExpirationHour = 18;
    private const int ExpirationMinute = 0;

    public static void CheckExpiry()
    {
        DateTime expiryDateTime = new DateTime(ExpirationDate.Year, ExpirationDate.Month, ExpirationDate.Day,
            ExpirationHour, ExpirationMinute, 0);
        TimeSpan remainingTime = expiryDateTime - DateTime.Now;
        if (remainingTime.TotalSeconds < 0)
        {
           // Application.Quit();
        }
        else
        {
            // Debug.Log("Not expired yet. Remaining time: " + remainingTime);
        }
    }
}
