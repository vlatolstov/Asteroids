namespace _Project.Runtime.Constants
{
    public static class AddressablesPrefabsPaths
    {
        public const string ShipView = "Views/ShipView";
        public const string UfoView = "Views/UfoView";
        public const string AsteroidView = "Views/AsteroidView";
        public const string AoeAttackView = "Views/AoeAttackView";
        public const string ProjectileView = "Views/ProjectileView";
        public const string AnimationView = "Views/AnimationView";
        public const string AudioSourceView = "Views/AudioSourceView";
        public const string BackgroundView = "Views/BackgroundView";
        public const string BGMView = "Views/BGMView";
        public const string HudView = "UI/HudView";
        public const string MenuView = "UI/MenuView";
    }
    
    public static class AddressablesConfigPaths
    {
        public static class Ads
        {
            public const string UnityAdsSettings = "Settings/AndroidAdsSettings";
        }
        
        public static class General
        {
            public const string AsteroidsSpawn = "Configs/General/AsteroidsSpawn";
            public const string UfoSpawn = "Configs/General/UfoSpawn";
            public const string GeneralSounds = "Configs/General/GeneralSounds";
            public const string GeneralVisuals = "Configs/General/GeneralVisuals";
        }

        public static class Weapons
        {
            public const string ShipGun = "Configs/Weapons/ShipGun";
            public const string ShipLaser = "Configs/Weapons/ShipLaser";
            public const string UfoBlaster = "Configs/Weapons/UfoBlaster";
        }

        public static class Attacks
        {
            public const string BlasterPulse = "Configs/Attacks/BlasterPulse";
            public const string Rocket = "Configs/Attacks/Rocket";
            public const string LaserAttack = "Configs/Attacks/LaserAttack";
        }
    }
}
