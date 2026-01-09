using UnityEngine;

namespace _Project.Runtime.Asteroid
{
    [CreateAssetMenu(fileName = "AsteroidsSpawnResource", menuName = "Resources/Asteroids Spawn")]
    public class AsteroidsSpawnResource : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _spritesVariations;

        public Sprite Sprite => _spritesVariations[Random.Range(0, _spritesVariations.Length)];
    }
}
