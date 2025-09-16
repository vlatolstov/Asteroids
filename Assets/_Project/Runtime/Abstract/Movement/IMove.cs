using UnityEngine;

namespace _Project.Runtime.Abstract.Movement
{
    public interface IMove
    {
        Vector2 Position { get; }
        Vector2 Velocity { get; }
        float AngleRadians { get; }
        void SetPose(Vector2 pos, Vector2 vel, float aRad);
    }
}