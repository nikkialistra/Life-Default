namespace Kernel.Types
{
    public interface IHittable
    {
        int Damage { get; }
        float Interval { get; }
    }
}