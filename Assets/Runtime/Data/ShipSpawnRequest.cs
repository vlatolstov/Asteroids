using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct ShipSpawnRequest : IEventData
    {
        public readonly Vector2 Position;

        public ShipSpawnRequest(Vector2 position)
        {
            Position = position;
        }
    }
    
    public readonly struct ShipDespawnRequest : IEventData
    {
        
    }

    public readonly struct ShipSpawned : IEventData
    {
        public readonly Vector2 Position;

        public ShipSpawned(Vector2 position)
        {
            Position = position;
        }
    }

    public struct ShipDestroyed : IEventData
    { }
}