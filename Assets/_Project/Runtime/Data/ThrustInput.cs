using _Project.Runtime.Abstract.MVP;

namespace _Project.Runtime.Data
{
    public struct ThrustInput : IStateData
    {
        public float Value;

        public ThrustInput(float value)
        {
            Value = value;
        }
    }

    public struct TurnInput : IStateData
    {
        public float Value;

        public TurnInput(float value)
        {
            Value = value;
        }
    }

    public struct AoeWeaponAttackPressed : IEventData
    { }

    public struct FireGunPressed : IEventData
    { }
}