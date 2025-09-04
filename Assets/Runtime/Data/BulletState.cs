using Runtime.Abstract.MVP;

namespace Runtime.Data
{
    public struct BulletState : IStateData
    {
        public float Cooldown;

        public BulletState(float cooldown)
        {
            Cooldown = cooldown;
        }
    }

    public struct LaserState : IStateData
    {
        public int Charges;
        public float Cooldown;

        public LaserState(int charges, float cooldown)
        {
            Charges = charges;
            Cooldown = cooldown;
        }
    }
}