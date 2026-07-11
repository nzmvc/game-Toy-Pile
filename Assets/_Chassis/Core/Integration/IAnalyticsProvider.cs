using System.Collections.Generic;

namespace Chassis.Core
{
    /// <summary>
    /// Contract for wrapping analytics engines (like Firebase, Unity Analytics, GameAnalytics).
    /// </summary>
    public interface IAnalyticsProvider
    {
        void LogEvent(string eventName, Dictionary<string, object> parameters = null);
    }
}
