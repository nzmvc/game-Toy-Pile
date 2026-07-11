# Event Schema

This document specifies the events and parameters sent to the analytics system. All analytics event names and parameters must follow this schema and be snake_case.

## Events Reference

### 1. Game Flow Events

#### `level_start`
Fired when a level starts.
* **Parameters**:
  * `level_id` (string/int): The ID of the loaded level.
  * `mechanic_id` (string): The identifier of the mechanic module (e.g. `tile_match`).
  * `attempt_no` (int): The attempt count for this level.

#### `level_complete`
Fired when the player wins/completes the level.
* **Parameters**:
  * `level_id` (string/int): The ID of the completed level.
  * `duration` (float): Duration in seconds from level start to win.
  * `moves` (int): Number of moves made.

#### `level_fail`
Fired when the player loses the level.
* **Parameters**:
  * `level_id` (string/int): The ID of the failed level.
  * `duration` (float): Duration in seconds from level start to lose.
  * `fail_reason` (string): Cause of failure (e.g., `board_full`, `time_out`).

---

### 2. Advertising Events

#### `ad_offer`
Fired when an ad (interstitial or rewarded) becomes eligible to show.
* **Parameters**:
  * `placement` (string): Placement label (e.g., `level_end`, `double_reward`, `add_slot`).
  * `ad_type` (string): Type of ad (`interstitial`, `rewarded`).

#### `ad_shown`
Fired when an ad starting event is successfully logged.
* **Parameters**:
  * `placement` (string): Location of the ad trigger.
  * `ad_type` (string): Type of ad (`interstitial`, `rewarded`).

#### `ad_reward`
Fired when the user watches a rewarded video fully and gets rewarded.
* **Parameters**:
  * `placement` (string): Location where rewarded value was claimed.
