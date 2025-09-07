using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "GeneralVisualsConfig", menuName = "Settings/General Visuals Config", order = 0)]
    public class GeneralVisualsConfig : ScriptableObject, IGeneralVisualsConfig
    {
        [SerializeField]
        private RuntimeAnimatorController _shipDestroyed;
        [SerializeField]
        private RuntimeAnimatorController _ufoDestroyed;
        [SerializeField]
        private RuntimeAnimatorController _asteroidDestroyed;

        public RuntimeAnimatorController ShipDestroyed => _shipDestroyed;
        public RuntimeAnimatorController UfoDestroyed => _ufoDestroyed;
        public RuntimeAnimatorController AsteroidDestroyed => _asteroidDestroyed;
    }
}