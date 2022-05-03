using Units.Enums;

namespace Units.MinimaxBehavior
{
    public class Player
    {
        public Player(Fraction fraction)
        {
            Fraction = fraction;
        }
        
        public Fraction Fraction { get; }
    }
}
