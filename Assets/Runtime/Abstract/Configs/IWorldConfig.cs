using UnityEngine;

namespace Runtime.Abstract.Configs
{
    public interface IWorldConfig
    {
        Rect WorldRect { get; }
        float WrapOffset { get; }
        Vector2 OffscreenPosition { get; }

        Rect ExpandedRect(float? customOffset = null)
        {
            float o = customOffset ?? WrapOffset;
            var r = WorldRect;
            r.xMin -= o;
            r.xMax += o;
            r.yMin -= o;
            r.yMax += o;
            return r;
        }
    }
}