using System;
using System.Collections.Generic;
using System.Linq;
using Units.Enums;

namespace Units.MinimaxBehavior
{
    public class Fight
    {
        private readonly Player _firstPlayer;
        private readonly Player _secondPlayer;

        private readonly PlayerFightStatuses _playerFightStatuses;
        
        private readonly Stack<HistoryFightState> _history;

        public Fight(Player firstPlayer, Player secondPlayer)
        {
            _firstPlayer = firstPlayer;
            _secondPlayer = secondPlayer;

            _history = new Stack<HistoryFightState>();
            
            State = FightState.FirstPlayerTurn;
        }
        
        public enum FightState
        {
            FirstPlayerTurn,
            SecondPlayerTurn,
            
            FirstPlayerVictory,
            SecondPlayerVictory,
            
            Draw
        }

        public Player FirstPlayer => _firstPlayer;
        public Player SecondPlayer => _secondPlayer;

        public FightState State { get; set; }

        public bool IsTerminal => State is FightState.FirstPlayerVictory or FightState.SecondPlayerVictory or FightState.Draw;

        public List<FightMove> GetPossibleMoves(Player player)
        {
            if (IsTerminal)
            {
                return new List<FightMove>();
            }

            return player.GetPossibleMoves();
        }

        public void MakeMove(Player player, FightMove fightMove)
        {
            if ((State == FightState.FirstPlayerTurn && player.Fraction != Fraction.Colonists) ||
            (State == FightState.SecondPlayerTurn && player.Fraction != Fraction.Enemies))
            {
                throw new InvalidOperationException("The turns belongs to another player");
            }

            if (IsTerminal)
            {
                throw new InvalidOperationException("The fight is finished");
            }
            
            _history.Push(new HistoryFightState(State, player.Fraction, fightMove));

            UpdatePlayerFightStatuses(player.Fraction, fightMove);

            State = DeduceState(fightMove, player.Fraction);
        }

        public void UndoMove()
        {
            if (_history.Count == 0)
            {
                throw new InvalidOperationException("Cannot undo when no moves were done");
            }

            var fightState = _history.Pop();
            
            UpdatePlayerFightStatuses(fightState.WhichMove, fightState.ReverseFightMove);

            State = fightState.State;
        }

        private void UpdatePlayerFightStatuses(Fraction fraction, FightMove fightMove)
        {
            if (fraction == Fraction.Colonists)
            {
                _playerFightStatuses.ChangeSecondPlayerHealth(fightMove.HitDamage);
                _playerFightStatuses.ChangeFirstPlayerHealth(fightMove.SelfDamage);
            }
            else
            {
                _playerFightStatuses.ChangeFirstPlayerHealth(fightMove.HitDamage);
                _playerFightStatuses.ChangeSecondPlayerHealth(fightMove.SelfDamage);
            }
        }

        private FightState DeduceState(FightMove fightMove, Fraction fraction)
        {
            if (IsSomeoneDefeated(out var fightState))
            {
                return fightState;
            }

            return fraction switch {
                Fraction.Colonists => FightState.SecondPlayerTurn,
                Fraction.Enemies => FightState.FirstPlayerTurn,
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }

        private bool IsSomeoneDefeated(out FightState secondPlayerVictory)
        {
            if (_playerFightStatuses.IsBothPlayersDefeated)
            {
                secondPlayerVictory = FightState.Draw;
                return true;
            }

            if (_playerFightStatuses.IsFirstPlayerDefeated)
            {
                secondPlayerVictory = FightState.FirstPlayerVictory;
                return true;
            }

            if (_playerFightStatuses.IsSecondPlayerDefeated)
            {
                secondPlayerVictory = FightState.SecondPlayerVictory;
                return true;
            }

            secondPlayerVictory = default;
            return false;
        }
    }
}
