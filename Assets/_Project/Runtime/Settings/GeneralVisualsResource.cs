using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "GeneralVisualsResource", menuName = "Resources/General Visuals", order = 0)]
    public class GeneralVisualsResource : ScriptableObject
    {
        [field: SerializeField]
        public RuntimeAnimatorController ShipDestroyed { get; private set; }

        [field: SerializeField]
        public RuntimeAnimatorController UfoDestroyed { get; private set; }

        [field: SerializeField]
        public RuntimeAnimatorController AsteroidDestroyed { get; private set; }
        
        [field: SerializeField]
        public Sprite ShipProjectileWeaponIcon { get; private set; }
        
        [field: SerializeField]
        public Sprite ShipAoeWeaponIcon { get; private set; }
    }
}
