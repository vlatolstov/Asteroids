using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct ShipSpawnRequest : IData
    {
        public readonly Vector2 Position;

        public ShipSpawnRequest(Vector2 position)
        {
            Position = position;
        }
    }
    
    public readonly struct ShipDespawnRequest : IData
    {
        
    }

    public readonly struct ShipSpawned : IData
    {
        public readonly Vector2 Position;

        public ShipSpawned(Vector2 position)
        {
            Position = position;
        }
    }

    public struct ShipDestroyed : IData
    { }
}