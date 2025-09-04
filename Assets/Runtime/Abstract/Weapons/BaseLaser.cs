using Runtime.Abstract.Configs;
using Runtime.Data;
using Runtime.Weapons;
using UnityEngine;

namespace Runtime.Abstract.Weapons
{
    public class BaseLaser : BaseWeapon<ILaserConfig>
    {
        public BaseLaser(ILaserConfig config, ProjectileHitResolver resolver) : base(config, resolver)
        { }

        public override bool TryFire()
        {
            throw new System.NotImplementedException();
        }

        protected override bool GetFireParams(out Vector2 origin, out Vector2 direction, out Vector2 inheritVelocity)
        {
            throw new System.NotImplementedException();
        }
    }
}