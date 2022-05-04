using UnityEngine;

namespace Units.MinimaxFightBehavior.FightAct
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

        public bool IsFirstPlayerWillBeDefeated(Player player)
        {
            return player.IfAttackWouldLeadDefeat(_firstPlayerHealth);
        }

        public bool IsSecondPlayerWillBeDefeated(Player player)
        {
            return player.IfAttackWouldLeadDefeat(_secondPlayerHealth);
        }

        public void ChangeFirstPlayerHealth(float delta)
        {
            _firstPlayerHealth -= delta;
        }

        public void ChangeSecondPlayerHealth(float delta)
        {
            _secondPlayerHealth -= delta;
        }

        public void ShowCurrentFightStatus()
        {
            Debug.Log($"First Health: {_firstPlayerHealth}, Second Health: {_secondPlayerHealth}");
        }
    }
}
