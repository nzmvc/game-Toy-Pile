using System.Collections.Generic;
using UnityEngine;

namespace Chassis.Core
{
    /// <summary>
    /// Development/fallback analytics provider that logs occurrences directly to the Unity Console.
    /// </summary>
    public class ConsoleAnalyticsProvider : IAnalyticsProvider
    {
        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            string paramText = "";
            if (parameters != null && parameters.Count > 0)
            {
                var sb = new System.Text.StringBuilder();
                sb.Append(" { ");
                foreach (var kvp in parameters)
                {
                    sb.Append($"{kvp.Key}: {kvp.Value}, ");
                }
                if (sb.Length > 3)
                {
                    sb.Length -= 2; // remove last comma and space
                }
                sb.Append(" }");
                paramText = sb.ToString();
            }

            Debug.Log($"[Analytics] <b>{eventName}</b>{paramText}");
        }
    }
}
