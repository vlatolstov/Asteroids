using UnityEngine;

namespace Runtime.Utils
{
    public static class WrapUtility
    {
        public static Vector2 Wrap(Vector2 p, Vector2 world)
        {
            p.x = Mod(p.x, world.x);
            p.y = Mod(p.y, world.y);
            return p;
        }

        public static float Mod(float v, float m)
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