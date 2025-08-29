using UnityEngine;

namespace Runtime.Abstract.Configs
{
    public interface IWorldConfig
    {
        Rect WorldRect { get; }
        Vector2 WorldSize { get; }
    }
}