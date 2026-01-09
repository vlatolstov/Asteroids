namespace _Project.Runtime.Constants
{
    public static class RemoteConfigKeys
    {
        public const string NumericConfigJson = "numeric_config_json";
    }

    public static class Config
    {
        public static class Ship
        {
            public const string Movement = "movement.ship";
        }

        public static class Asteroids
        {
            public const string Movement = "movement.asteroid";
            public const string Spawn = "asteroidsSpawn";
        }

        public static class Ufo
        {
            public const string Movement = "movement.ufo";
            public const string Spawn = "ufoSpawn";
            public const string Chasing = "chasingUfo";
        }

        public static class Background
        {
            public const string Jitter = "backgroundJitter";
        }

        public const string Score = "score";

        public static class Weapon
        {
            public const string ShipGun = "weapons.shipGun";
            public const string ShipLaser = "weapons.shipLaser";
            public const string UfoBlaster = "weapons.ufoBlaster";
        }

        public static class Attack
        {
            public const string BlasterPulse = "weapons.attacks.blasterPulse";
            public const string Rocket = "weapons.attacks.rocket";
            public const string LaserAttack = "weapons.attacks.laserAttack";
        }
    }
}
