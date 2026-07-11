using UnityEngine;

namespace Chassis.Core
{
    /// <summary>
    /// ScriptableObject representing level configuration. JSON-compatible serialization.
    /// Each game mechanic will store its specific level properties inside `jsonData`.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Chassis/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int levelId;
        public string mechanicId;
        public int difficulty;

        [TextArea(5, 15)]
        public string jsonData;

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public virtual void FromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}
