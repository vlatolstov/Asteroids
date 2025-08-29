using UnityEngine;

namespace Runtime.Utils
{
    public static class WrapUtility
    {
        public static Vector2 Wrap(Vector2 p, Rect worldRect)
        {
            p.x = Mod(p.x - worldRect.xMin, worldRect.width)  + worldRect.xMin;
            p.y = Mod(p.y - worldRect.yMin, worldRect.height) + worldRect.yMin;
            return p;
        }

        private static float Mod(float v, float m)
        {
            if (m <= 0f)
            {
                return 0f;
            }

            float r = v % m;
            
            if (r < 0f)
            {
                r += m;
            }

            return r;
        }
    }
}