using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct ShipSpawnRequest : IEventData
    { }

    public readonly struct ShipSpawnCommand : IEventData
    {
        public readonly Vector2 Position;

        public ShipSpawnCommand(Vector2 position)
        {
            Position = position;
        }
    }

    public readonly struct ShipDespawnRequest : IEventData
    {
        public readonly uint ViewId;

        public ShipDespawnRequest(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct ShipDespawnCommand : IEventData
    {
        public readonly uint ViewId;

        public ShipDespawnCommand(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct ShipSpawned : IStateData
    {
        public readonly bool Status;
        public readonly uint ViewId;
        public readonly Vector2 Position;

        public ShipSpawned(bool status, uint viewId, Vector2 position)
        {
            ViewId = viewId;
            Position = position;
            Status = status;
        }
    }

    public struct ShipDestroyed : IStateData
    {
        public readonly uint ViewId;
        public readonly Vector2 Position;
        public readonly Vector2 Scale;

        public ShipDestroyed(uint viewId, Vector2 position, Vector2 scale)
        {
            ViewId = viewId;
            Position = position;
            Scale = scale;
        }
    }
}