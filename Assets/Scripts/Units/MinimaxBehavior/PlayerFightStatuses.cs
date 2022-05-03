namespace Units.MinimaxBehavior
{
    public class PlayerFightStatuses
    {
        private float _firstPlayerHealth;
        private float _secondPlayerHealth;

        public PlayerFightStatuses(float firstPlayerHealth, float secondPlayerHealth)
        {
            _firstPlayerHealth = firstPlayerHealth;
            _secondPlayerHealth = secondPlayerHealth;
        }
        
        public bool IsFirstPlayerDefeated => _firstPlayerHealth <= 0;
        public bool IsSecondPlayerDefeated => _secondPlayerHealth <= 0;
        
        public bool IsBothPlayersDefeated => IsFirstPlayerDefeated && IsSecondPlayerDefeated;

        public void ChangeFirstPlayerHealth(float delta)
        {
            _firstPlayerHealth += delta;
        }
        
        public void ChangeSecondPlayerHealth(float delta)
        {
            _secondPlayerHealth += delta;
        }
    }
}
