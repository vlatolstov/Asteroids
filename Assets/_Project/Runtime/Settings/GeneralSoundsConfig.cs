using _Project.Runtime.Abstract.Configs;
using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "GeneralSoundsConfig", menuName = "Settings/General Sounds Config", order = 0)]
    public class GeneralSoundsConfig : ScriptableObject
    {
        [field: SerializeField]
        public AudioClip ShipSpawn { get; private set; }
    }
}