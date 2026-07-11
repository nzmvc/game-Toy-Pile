namespace Chassis.Core
{
    /// <summary>
    /// Lifecycle interface that all game mechanics must implement.
    /// The GameManager initializes and runs the active mechanic.
    /// </summary>
    public interface IGameMechanic
    {
        void Initialize(LevelData levelData);
        void StartLevel();
        void Tick(float dt);
        void EndLevel(LevelResult result);
    }

    /// <summary>
    /// Event publication when a level is completed successfully.
    /// </summary>
    public struct LevelCompletedEvent
    {
        public LevelResult result;
    }

    /// <summary>
    /// Event publication when a level has failed.
    /// </summary>
    public struct LevelFailedEvent
    {
        public LevelResult result;
    }

    /// <summary>
    /// Event publication to notify UI or progress trackers about progress changes.
    /// </summary>
    public struct LevelProgressChangedEvent
    {
        public float progress; // Value range: [0.0..1.0]
    }
}
