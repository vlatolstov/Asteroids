using UnityEngine;

namespace _Project.Runtime.Ufo
{
    [CreateAssetMenu(fileName = "UfoSpawnResource", menuName = "Resources/UFO Spawn")]
    public sealed class UfoSpawnResource : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _sprites;

        public Sprite Sprite => _sprites[Random.Range(0, _sprites.Length)];
    }
}
