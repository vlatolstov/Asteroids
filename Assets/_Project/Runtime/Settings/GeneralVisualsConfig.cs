using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "GeneralVisualsConfig", menuName = "Settings/General Visuals Config", order = 0)]
    public class GeneralVisualsConfig : ScriptableObject
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