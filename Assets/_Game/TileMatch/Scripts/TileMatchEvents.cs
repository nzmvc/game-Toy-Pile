using UnityEngine;

namespace Game.TileMatch
{
    /// <summary>
    /// Event published when the collection bar reaches the near-miss capacity threshold.
    /// Used for triggering visual tension UI/juice shakes.
    /// </summary>
    public struct NearMissEvent
    {
    }

    /// <summary>
    /// Event published when 3 tiles of the same type are successfully matched in the bar and popped.
    /// </summary>
    public struct MatchPoppedEvent
    {
        public int typeId;
        public Vector3 popPosition;
        public int scoreAdded;
    }
}
