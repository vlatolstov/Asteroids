using UnityEngine;

namespace Runtime.Abstract.Weapons
{
    public interface IFireParamsSource
    {
        bool TryGetFireParams(out Vector2 origin, out Vector2 direction, out Vector2 inheritVelocity, out int layer);
    }
}