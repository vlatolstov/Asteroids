using Runtime.Abstract.MVP;

namespace Runtime.Data
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

    public struct FireLaserPressed : IEventData
    { }

    public struct FireGunPressed : IEventData
    { }
}