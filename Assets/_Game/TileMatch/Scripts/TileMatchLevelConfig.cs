using System;
using System.Collections.Generic;

namespace Game.TileMatch
{
    /// <summary>
    /// Config layout parsed from LevelData.jsonData for the Tile Match mechanic.
    /// Represents the set of items (type IDs and their respective counts) to spawn in the level.
    /// </summary>
    [Serializable]
    public class TileMatchLevelConfig
    {
        public List<TileItemConfig> items = new List<TileItemConfig>();
    }

    /// <summary>
    /// Represents a specific object type ID and the count of objects of that type to be spawned.
    /// </summary>
    [Serializable]
    public class TileItemConfig
    {
        public int typeId;
        public int count;
    }
}
