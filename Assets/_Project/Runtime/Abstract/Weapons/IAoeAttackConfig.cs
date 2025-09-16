using System;

namespace _Project.Runtime.Abstract.Weapons
{
    [Serializable]
    public enum AoeAttachMode
    {
        FollowEmitter,
        Static
    }
    
    public interface IAoeAttackConfig
    {
        float Length { get; }
        float Width { get; }
        float Duration { get; }
        AoeAttachMode Mode { get; }
    }
}