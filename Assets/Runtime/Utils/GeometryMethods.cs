using UnityEngine;

namespace Runtime.Utils
{
    public static class GeometryMethods
    {
        public static void SetWorldSizeOfChildObject(SpriteRenderer sr, float worldWidth, float worldLength)
        {
            if (!sr || !sr.sprite) return;

            Vector2 baseSize = sr.sprite.bounds.size;
            Vector3 parentScale = sr.transform.parent ? sr.transform.parent.lossyScale : Vector3.one;

            float sx = worldWidth  / (baseSize.x * parentScale.x);
            float sy = worldLength / (baseSize.y * parentScale.y);

            sr.transform.localScale = new Vector3(sx, sy, 1f);
        }
        
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
        
        public static Vector2 RotateVector(Vector2 v, float angRad)
        {
            float s = Mathf.Sin(angRad);
            float c = Mathf.Cos(angRad);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
        }
        
        public static Vector2 AngleToDir(float angRad) => new(-Mathf.Sin(angRad), Mathf.Cos(angRad));
        
        public static float SignedAngleRad(Vector2 from, Vector2 to)
        {
            float s = from.x * to.y - from.y * to.x;
            float c = Vector2.Dot(from, to);
            return Mathf.Atan2(s, c);
        }

        public static Vector2 ShortestWrappedDelta(Vector2 from, Vector2 to, Rect world)
        {
            float w = world.width;
            float h = world.height;
            float dx = to.x - from.x;
            float dy = to.y - from.y;

            if (dx >  w * 0.5f) dx -= w; else if (dx < -w * 0.5f) dx += w;
            if (dy >  h * 0.5f) dy -= h; else if (dy < -h * 0.5f) dy += h;

            return new Vector2(dx, dy);
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