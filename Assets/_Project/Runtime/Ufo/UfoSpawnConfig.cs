using UnityEngine;

namespace _Project.Runtime.Ufo
{
    [CreateAssetMenu(fileName = "UfoSpawnConfig", menuName = "Settings/UFO Spawn Config")]
    public sealed class UfoSpawnConfig : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _sprites;

        public Sprite Sprite => _sprites[Random.Range(0, _sprites.Length)];
    }
}
