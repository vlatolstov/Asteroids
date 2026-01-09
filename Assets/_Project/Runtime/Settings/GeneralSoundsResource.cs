using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "GeneralSoundsResource", menuName = "Resources/General Sounds", order = 0)]
    public class GeneralSoundsResource : ScriptableObject
    {
        [field: SerializeField]
        public AudioClip ShipSpawn { get; private set; }
    }
}
