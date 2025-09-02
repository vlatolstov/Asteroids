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
        
        public static Vector2 Wrap(Vector2 p, Rect r, float offset)
        {
            if (offset < 0f) offset = 0f;

            float xMin = r.xMin - offset;
            float xMax = r.xMax + offset;
            float yMin = r.yMin - offset;
            float yMax = r.yMax + offset;

            float w = xMax - xMin;
            float h = yMax - yMin;

            float x = p.x, y = p.y;
            if (x < xMin) x += w; else if (x > xMax) x -= w;
            if (y < yMin) y += h; else if (y > yMax) y -= h;

            return new Vector2(x, y);
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