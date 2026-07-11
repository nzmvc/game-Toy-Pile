namespace Chassis.Core
{
    /// <summary>
    /// Type-safe constants for all analytics events and parameter keys.
    /// This prevents typos and enforces the snake_case rules defined in docs/EventSchema.md.
    /// </summary>
    public static class AnalyticsEvents
    {
        // Event Names
        public const string LevelStart = "level_start";
        public const string LevelComplete = "level_complete";
        public const string LevelFail = "level_fail";
        public const string AdOffer = "ad_offer";
        public const string AdShown = "ad_shown";
        public const string AdReward = "ad_reward";

        // Parameter Keys
        public static class Params
        {
            public const string LevelId = "level_id";
            public const string MechanicId = "mechanic_id";
            public const string AttemptNo = "attempt_no";
            public const string Duration = "duration";
            public const string Moves = "moves";
            public const string FailReason = "fail_reason";
            public const string Placement = "placement";
            public const string AdType = "ad_type";
        }
    }
}
