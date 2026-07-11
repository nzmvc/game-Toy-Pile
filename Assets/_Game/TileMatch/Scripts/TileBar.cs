using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chassis.Core;
using Chassis.Juice;

namespace Game.TileMatch
{
    /// <summary>
    /// Component managing the 7 collection slots. Handles tile insertions (sorting identical type IDs adjacent),
    /// shifting animations, match-3 pops, and near-miss/failed game state checks.
    /// </summary>
    public class TileBar : MonoBehaviour
    {
        [Tooltip("The 7 slot positions (visual anchors relative to camera) where collected tiles rest.")]
        [SerializeField] private List<Transform> slots = new List<Transform>();

        private List<TileObject> _activeTiles = new List<TileObject>();
        private int _maxSlots = 7;

        public int ActiveCount => _activeTiles.Count;

        public void Initialize(int maxSlots)
        {
            _maxSlots = maxSlots;
            _activeTiles.Clear();
        }

        public void ExpandMaxSlots(int newMaxSlots)
        {
            _maxSlots = newMaxSlots;
            Debug.Log($"[TileBar] Expanded max slots to: {_maxSlots}");
        }

        /// <summary>
        /// Attempts to add a tile to the bar, sorting it adjacent to other tiles of the same type.
        /// </summary>
        public bool TryAddTile(
            TileObject tile, 
            float flyDuration, 
            Action<TileObject, TileObject, TileObject> onMatchCallback, 
            Action onFailCallback, 
            Action onNearMissCallback)
        {
            if (_activeTiles.Count >= _maxSlots)
            {
                return false;
            }

            int insertIndex = GetInsertIndex(tile.TypeId);
            _activeTiles.Insert(insertIndex, tile);

            // Record target slots for all tiles before match checks, so we know where matched tiles fly
            List<Transform> temporaryTargetSlots = new List<Transform>();
            for (int i = 0; i < _activeTiles.Count; i++)
            {
                temporaryTargetSlots.Add(slots[Mathf.Min(i, slots.Count - 1)]);
            }

            // Check for match
            bool didMatch = CheckMatches(out List<TileObject> matchedTiles, out List<int> matchedIndices);

            if (didMatch)
            {
                // Animate matched tiles flying to their temporary target slots before popping
                for (int m = 0; m < matchedTiles.Count; m++)
                {
                    int indexBeforeRemoval = matchedIndices[m];
                    Transform targetSlot = temporaryTargetSlots[indexBeforeRemoval];

                    if (matchedTiles[m] == tile)
                    {
                        matchedTiles[m].Collect(targetSlot, flyDuration, null);
                    }
                    else
                    {
                        matchedTiles[m].MoveToSlot(targetSlot, flyDuration);
                    }
                }

                // Handle destruction and visual effects with a slight delay so they arrive at slots first
                StartCoroutine(PopMatchedTilesCoroutine(matchedTiles, flyDuration, onMatchCallback));
            }

            // Animate remaining active tiles to their correct, shifted slots
            for (int i = 0; i < _activeTiles.Count; i++)
            {
                TileObject activeTile = _activeTiles[i];
                Transform targetSlot = slots[Mathf.Min(i, slots.Count - 1)];

                if (activeTile == tile)
                {
                    activeTile.Collect(targetSlot, flyDuration, null);
                }
                else
                {
                    activeTile.MoveToSlot(targetSlot, flyDuration);
                }
            }

            // Evaluate state conditions
            if (_activeTiles.Count == 6)
            {
                var gameConfig = ServiceLocator.Get<GameConfig>();
                float strength = gameConfig != null ? gameConfig.nearMissBarShakeStrength : 0.15f;
                float duration = gameConfig != null ? gameConfig.nearMissBarShakeDuration : 0.5f;

                // Shake the bar transform physically for visual tension warning
                TweenHelper.ShakePosition(JuicePlayer.Instance, transform, strength, duration);

                // Play near miss warning audio/vfx
                JuicePlayer.Instance.PlayNearMiss(transform.position);

                onNearMissCallback?.Invoke();
            }
            else if (_activeTiles.Count >= _maxSlots)
            {
                onFailCallback?.Invoke();
            }

            return true;
        }

        private IEnumerator PopMatchedTilesCoroutine(
            List<TileObject> matchedTiles, 
            float delay, 
            Action<TileObject, TileObject, TileObject> onMatchCallback)
        {
            yield return new WaitForSeconds(delay);

            if (matchedTiles.Count == 3 && matchedTiles[0] != null)
            {
                onMatchCallback?.Invoke(matchedTiles[0], matchedTiles[1], matchedTiles[2]);
            }
        }

        private bool CheckMatches(out List<TileObject> matchedTiles, out List<int> matchedIndices)
        {
            matchedTiles = new List<TileObject>();
            matchedIndices = new List<int>();

            for (int i = 0; i <= _activeTiles.Count - 3; i++)
            {
                if (_activeTiles[i].TypeId == _activeTiles[i + 1].TypeId &&
                    _activeTiles[i].TypeId == _activeTiles[i + 2].TypeId)
                {
                    matchedTiles.Add(_activeTiles[i]);
                    matchedTiles.Add(_activeTiles[i + 1]);
                    matchedTiles.Add(_activeTiles[i + 2]);

                    matchedIndices.Add(i);
                    matchedIndices.Add(i + 1);
                    matchedIndices.Add(i + 2);

                    _activeTiles.RemoveAt(i + 2);
                    _activeTiles.RemoveAt(i + 1);
                    _activeTiles.RemoveAt(i);

                    return true;
                }
            }
            return false;
        }

        private int GetInsertIndex(int typeId)
        {
            int insertIndex = -1;
            for (int i = 0; i < _activeTiles.Count; i++)
            {
                if (_activeTiles[i].TypeId == typeId)
                {
                    insertIndex = i;
                }
            }

            if (insertIndex != -1)
            {
                return insertIndex + 1;
            }
            return _activeTiles.Count;
        }

        public void ClearBar()
        {
            foreach (var tile in _activeTiles)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
            _activeTiles.Clear();
        }

        public bool IsEmpty()
        {
            return _activeTiles.Count == 0;
        }
    }
}
