using Runtime.Abstract.Weapons;
using Runtime.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Runtime.Contexts.Global
{
    [CreateAssetMenu(fileName = "WeaponsInstaller", menuName = "Installers/Weapons Installer")]
    public class WeaponsInstaller : ScriptableObjectInstaller
    {
        public enum WeaponId
        {
            ShipGun,
            ShipLaser,
            UfoGun
        }
        
        [SerializeField]
        private ProjectileWeaponConfig _shipGun;
        
        [FormerlySerializedAs("_shipLaser")]
        [SerializeField]
        private AoeWeaponConfig _shipAoeWeapon;
        
        [SerializeField]
        private ProjectileWeaponConfig _ufoGun;
        
        public override void InstallBindings()
        {
            Container
                .Bind<IProjectileWeaponConfig>()
                .WithId(WeaponId.ShipGun)
                .FromInstance(_shipGun);
        }
    }
}