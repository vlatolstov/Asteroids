namespace _Project.Runtime.Abstract.Movement
{
    public interface IMotorInput
    {
        void SetThrust(float thrust);

        void SetTurnAxis(float turnAxis);
    }
}