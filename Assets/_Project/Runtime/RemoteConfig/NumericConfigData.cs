using System;
using UnityEngine;

namespace _Project.Runtime.RemoteConfig
{
    [Serializable]
    public sealed class NumericConfigRoot
    {
        public MovementGroup Movement;
        public BackgroundJitterData BackgroundJitter;
        public ChasingUfoData ChasingUfo;
        public AsteroidsSpawnData AsteroidsSpawn;
        public UfoSpawnData UfoSpawn;
        public ScoreData Score;
        public WeaponsGroup Weapons;
    }

    [Serializable]
    public sealed class MovementGroup
    {
        public MovementConfigData Ship;
        public MovementConfigData Asteroid;
        public MovementConfigData Ufo;
    }

    [Serializable]
    public sealed class MovementConfigData
    {
        public float Acceleration;
        public float MaxSpeed;
        public float TurnSpeed;
        public float LinearDamping;
        public bool IsWrappedByWorldBounds;
    }

    [Serializable]
    public sealed class BackgroundJitterData
    {
        public float SlowAmplitude;
        public float SlowFrequency;
        public float JitterAmplitude;
        public float JitterFrequency;
        public float ParallaxStrength;
        public float ParallaxResponse;
        public float ParallaxMax;
        public float SmoothTime;
        public float MaxOffset;
        public bool UseUnscaledTime;
    }

    [Serializable]
    public sealed class ChasingUfoData
    {
        public float TurnKp;
        public float TurnKd;
        public float ThrustKp;
        public float MaxThrust;
        public float AimToleranceDegrees;
        public float MaxLeadSeconds;
        public float MaxFireDistance;
    }

    [Serializable]
    public sealed class AsteroidsSpawnData
    {
        public float RotationMinDeg;
        public float RotationMaxDeg;
        public float Interval;
        public float LargeScale;
        public float EdgeOffset;
        public float EntrySpeedMin;
        public float EntrySpeedMax;
        public float EntryAngleJitterDeg;
        public float SmallScale;
        public int SmallSplitMin;
        public int SmallSplitMax;
        public float SmallSpeedMin;
        public float SmallSpeedMax;
    }

    [Serializable]
    public sealed class UfoSpawnData
    {
        public float Scale;
        public float InitialDelay;
        public float Interval;
        public int MaxAlive;
        public float EdgeOffset;
        public float Speed;
        public float EntryAngleJitterDeg;
    }

    [Serializable]
    public sealed class ScoreData
    {
        public int LargeAsteroid;
        public int SmallAsteroid;
        public int Ufo;
    }

    [Serializable]
    public sealed class WeaponsGroup
    {
        public ProjectileWeaponData ShipGun;
        public AoeWeaponData ShipLaser;
        public ProjectileWeaponData UfoBlaster;
        public AttacksGroup Attacks;
    }

    [Serializable]
    public sealed class ProjectileWeaponData
    {
        public float WeaponCooldown;
        public float MuzzleOffset;
        public float Spread;
        public int BulletsPerShot;
        public float BulletsInterval;
    }

    [Serializable]
    public sealed class AoeWeaponData
    {
        public float WeaponCooldown;
        public float MuzzleOffset;
        public int Charges;
        public float ChargeRate;
    }

    [Serializable]
    public sealed class AttacksGroup
    {
        public ProjectileAttackData BlasterPulse;
        public ProjectileAttackData Rocket;
        public AoeAttackData LaserAttack;
    }

    [Serializable]
    public sealed class ProjectileAttackData
    {
        public Vector2Data Size;
        public float Speed;
        public float Lifetime;
    }

    [Serializable]
    public sealed class AoeAttackData
    {
        public float Length;
        public float Width;
        public float Duration;
        public int AttachMode;
    }

    [Serializable]
    public struct Vector2Data
    {
        public float X;
        public float Y;

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
