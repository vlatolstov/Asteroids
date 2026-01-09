using UnityEngine;

namespace _Project.Runtime.Asteroid
{
    [CreateAssetMenu(fileName = "AsteroidsSpawnConfig", menuName = "Settings/Asteroids Spawn Config")]
    public class AsteroidsSpawnConfig : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _spritesVariations;

        public Sprite Sprite => _spritesVariations[Random.Range(0, _spritesVariations.Length)];
    }
}
