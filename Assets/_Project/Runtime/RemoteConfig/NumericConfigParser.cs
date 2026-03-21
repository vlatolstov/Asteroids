using System;
using System.Collections.Generic;
using _Project.Runtime.Constants;
using UnityEngine;

namespace _Project.Runtime.RemoteConfig
{
    public sealed class NumericConfigParser
    {
        public Dictionary<string, object> Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, object>();
            }

            NumericConfigRoot root;
            try
            {
                root = JsonUtility.FromJson<NumericConfigRoot>(json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[RemoteConfig] Failed to parse numeric config JSON: {ex}");
                return new Dictionary<string, object>();
            }

            return BuildMap(root);
        }

        private static Dictionary<string, object> BuildMap(NumericConfigRoot root)
        {
            var map = new Dictionary<string, object>();

            if (root == null)
            {
                return map;
            }

            TryAdd(map, Config.Ship.Movement, root.Movement?.Ship);
            TryAdd(map, Config.Asteroids.Movement, root.Movement?.Asteroid);
            TryAdd(map, Config.Ufo.Movement, root.Movement?.Ufo);
            TryAdd(map, Config.Background.Jitter, root.BackgroundJitter);
            TryAdd(map, Config.Asteroids.Spawn, root.AsteroidsSpawn);
            TryAdd(map, Config.Ufo.Spawn, root.UfoSpawn);
            TryAdd(map, Config.Ufo.Chasing, root.ChasingUfo);
            TryAdd(map, Config.Score, root.Score);
            TryAdd(map, Config.Weapon.ShipGun, root.Weapons?.ShipGun);
            TryAdd(map, Config.Weapon.ShipLaser, root.Weapons?.ShipLaser);
            TryAdd(map, Config.Weapon.ShipPowerShield, root.Weapons?.ShipPowerShield);
            TryAdd(map, Config.Weapon.UfoBlaster, root.Weapons?.UfoBlaster);
            TryAdd(map, Config.Attack.BlasterPulse, root.Weapons?.Attacks?.BlasterPulse);
            TryAdd(map, Config.Attack.Rocket, root.Weapons?.Attacks?.Rocket);
            TryAdd(map, Config.Attack.LaserAttack, root.Weapons?.Attacks?.LaserAttack);
            TryAdd(map, Config.Attack.PowerShieldAttack, root.Weapons?.Attacks?.PowerShieldAttack);

            return map;
        }

        private static void TryAdd<T>(Dictionary<string, object> map, string key, T value) where T : class
        {
            if (value != null)
            {
                map[key] = value;
            }
        }
    }
}
