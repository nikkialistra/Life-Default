namespace Units.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float value);
        void TakeDamageContinuously(float value, float interval, float time);
        void StopTakingDamage();
    }
}
