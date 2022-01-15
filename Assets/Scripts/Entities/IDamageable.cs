namespace Entities
{
    public interface IDamageable
    {
        void TakeDamage(int value);
        void TakeDamageContinuously(int value, float interval, float time);
        void StopTakingDamage();
    }
}
