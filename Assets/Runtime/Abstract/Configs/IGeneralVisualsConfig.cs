using UnityEngine;

namespace Runtime.Abstract.Configs
{
    public interface IGeneralVisualsConfig
    {
        RuntimeAnimatorController ShipDestroyed { get; }
        RuntimeAnimatorController UfoDestroyed { get; }
        RuntimeAnimatorController AsteroidDestroyed { get; }
    }
}