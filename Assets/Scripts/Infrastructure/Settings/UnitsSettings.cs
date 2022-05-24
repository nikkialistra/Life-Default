using System;

namespace Infrastructure.Settings
{
    [Serializable]
    public class UnitsSettings
    {
        public float HealthFractionToDecreaseRecoverySpeed = 0.8f;
        public float RecoveryHealthDelayAfterHit = 5f;
        public float RecoverySpeedRestoreSpeed = 0.1f;
    }
}
