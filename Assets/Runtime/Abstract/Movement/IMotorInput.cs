namespace Runtime.Abstract.Movement
{
    public interface IMotorInput
    {
        void SetControls(float thrust, float turnAxis);
    }
}