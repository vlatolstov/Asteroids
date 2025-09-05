using Runtime.Abstract.Configs;
using Runtime.Abstract.Weapons;

namespace Runtime.Weapons
{
    public class AoeWeapon : BaseWeapon<IAoeWeaponConfig>
    {
        public AoeWeapon(IAoeWeaponConfig config, IFireParamsSource source) : base(config, source)
        { }
    
        public override bool TryAttack()
        {
            return true;
        }
    }
}