using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "GeneralSoundsConfig", menuName = "Settings/General Sounds Config", order = 0)]
    public class GeneralSoundsConfig : ScriptableObject, IGeneralSoundsConfig
    {
        [SerializeField]
        private AudioClip _shipSpawn;

        public AudioClip ShipSpawn => _shipSpawn;
    }
}