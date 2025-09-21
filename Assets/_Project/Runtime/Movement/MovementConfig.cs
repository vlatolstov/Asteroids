using UnityEngine;

namespace _Project.Runtime.Movement
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Settings/MovementConfig", order = 0)]
    public class MovementConfig : ScriptableObject
    {
        [field: SerializeField]
        public float Acceleration { get; private set; }

        [field: SerializeField]
        public float MaxSpeed { get; private set; }

        [field: SerializeField]
        public float TurnSpeed { get; private set; }

        [field: SerializeField]
        public float LinearDamping { get; private set; }

        [field: SerializeField]
        public bool IsWrappedByWorldBounds { get; private set; }
    }
}