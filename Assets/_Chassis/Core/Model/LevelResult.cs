using System;
using UnityEngine;

namespace Chassis.Core
{
    /// <summary>
    /// Data structure representing the outcome of playing a level.
    /// JSON-compatible to ease serialization and remote logging.
    /// </summary>
    [Serializable]
    public class LevelResult
    {
        public int levelId;
        public bool isSuccess;
        public float duration;
        public int movesCount;
        public int score;
        public string failReason;

        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public static LevelResult FromJson(string json)
        {
            return JsonUtility.FromJson<LevelResult>(json);
        }
    }
}
