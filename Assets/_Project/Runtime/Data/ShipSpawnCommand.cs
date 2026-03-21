using UnityEngine;

namespace _Project.Runtime.Data
{
    public readonly struct ShipSpawnCommand
    {
        public readonly Vector2 Position;
        public readonly int SpawnNumber;
        public readonly bool ShouldActivateShield;

        public ShipSpawnCommand(Vector2 position, int spawnNumber, bool shouldActivateShield)
        {
            Position = position;
            SpawnNumber = spawnNumber;
            ShouldActivateShield = shouldActivateShield;
        }
    }
}
